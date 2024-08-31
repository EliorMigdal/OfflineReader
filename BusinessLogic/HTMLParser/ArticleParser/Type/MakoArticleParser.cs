using System.Text;
using BusinessLogic.Article.Content;
using BusinessLogic.Article.Content.TextType;
using BusinessLogic.Article.Partials;
using HtmlAgilityPack;

namespace BusinessLogic.HTMLParser.ArticleParser.Type;

public sealed class MakoArticleParser : IArticleParser
{
    private static readonly object rm_CreationLock = new();
    private readonly object rm_ParsingLock = new();
    private static MakoArticleParser? m_Instance;
    public static MakoArticleParser Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new MakoArticleParser();

                return m_Instance;   
            }
        }
    }
    
    private MakoArticleParser() {}
    
    [Obsolete]
    public InnerArticle ParseArticleHTML(string i_HTML)
    {
        lock (rm_ParsingLock)
        {
            HtmlDocument htmlDocument = new();
            InnerArticle article = new();
            int numOfImages = 1;

            htmlDocument.LoadHtml(i_HTML);
            extractArticleTitle(article, htmlDocument);
            extractArticleDescription(article, htmlDocument);
            extractArticleAuthorInfo(article, htmlDocument);
            extractArticleDatesInfo(article, htmlDocument);
            extractHeaderImage(article, htmlDocument, ref numOfImages);
            extractArticleBody(article, htmlDocument, ref numOfImages);

            return article;
        }
    }

    private void extractArticleTitle(InnerArticle io_Article, HtmlDocument i_HTML)
    {
        HtmlNode headlineNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='headline']");

        if (headlineNode == null || headlineNode.Attributes["content"] == null) return;
        string headline = HtmlEntity.DeEntitize(headlineNode.Attributes["content"].Value);
        io_Article.Title = headline;
    }

    private void extractArticleDescription(InnerArticle io_Article, HtmlDocument i_HTML)
    {
        HtmlNode descriptionNode = i_HTML.DocumentNode.SelectSingleNode("//meta[@name='description']");

        if (descriptionNode == null) return;
        string descriptionContent = descriptionNode.GetAttributeValue("content", string.Empty);

        if (string.IsNullOrEmpty(descriptionContent)) return;
        string descriptionText = HtmlEntity.DeEntitize(descriptionContent);
        io_Article.Description = descriptionText;
    }

    private void extractArticleAuthorInfo(InnerArticle io_Article, HtmlDocument i_HTML)
    {
        HtmlNode authorInfoNode = i_HTML.DocumentNode.SelectSingleNode("//div[@class='writer-data']");
        if (authorInfoNode == null) return;
        io_Article.Author = new Author();
        HtmlNode authorImageNode = authorInfoNode.SelectSingleNode(".//img[@src]");
        HtmlNodeCollection authorNameNodes = authorInfoNode.SelectNodes(".//a[contains(@href, 'Editor-')] | .//span[@itemprop='author' and @content] | .//span[@class='source']");

        if (authorNameNodes != null)
        {
            string authorName = string.Empty;

            foreach (HtmlNode nameNode in authorNameNodes)
            {
                authorName = HtmlEntity.DeEntitize(nameNode.Attributes.Contains("content") ? 
                    nameNode.GetAttributeValue("content", string.Empty) : nameNode.InnerText);

                if (!string.IsNullOrEmpty(authorName))
                    break;
            }

            io_Article.Author.Name = authorName;
        }

        if (authorImageNode is null) return;
        string authorImageSrc = authorImageNode.GetAttributeValue("src", string.Empty);
        io_Article.Author.ImageSource = authorImageSrc;
    }

    private void extractArticleDatesInfo(InnerArticle io_Article, HtmlDocument i_HTML)
    {
        HtmlNode datePublishedNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='datePublished']");
        HtmlNode dateModifiedNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='dateModified']");

        if (datePublishedNode != null)
        {
            DateTime datePublished = DateTime.Parse(datePublishedNode.GetAttributeValue("content", string.Empty));
            io_Article.PublishedDate = DateTime.Parse(datePublished.ToString("dd-MM-yyyy HH:mm"));
        }

        if (dateModifiedNode == null) return;
        DateTime dateModified = DateTime.Parse(dateModifiedNode.GetAttributeValue("content", string.Empty));
        io_Article.LastUpdated = DateTime.Parse(dateModified.ToString("dd-MM-yyyy HH:mm"));
    }

    private void extractHeaderImage(InnerArticle io_Article, HtmlDocument i_HTML, ref int i_NumOfImages)
    {
        HtmlNode initialImageNode = i_HTML.DocumentNode.SelectSingleNode("//section[contains(@class, 'article-header')]/figure");

        if (initialImageNode is null) return;
        
        HtmlNode imgNode = initialImageNode.SelectSingleNode(".//img");
        HtmlNode captionNode = initialImageNode.SelectSingleNode(".//figcaption");

        if (imgNode is null) return;
        
        string src = imgNode.GetAttributeValue("src", null);
        string description = captionNode != null ? HtmlEntity.DeEntitize(captionNode.InnerText) : string.Empty;

        if (string.IsNullOrEmpty(src)) return;
        
        io_Article.BodyContents.Add(new ImageContent(src, i_NumOfImages++));

        if (!string.IsNullOrEmpty(description))
        {
            io_Article.BodyContents.Add(new ImageCredit(description));
        }
    }

    private void extractArticleBody(InnerArticle io_Article, HtmlDocument i_HTML, ref int i_NumOfImages)
    {
        HtmlNode articleBodyNode = i_HTML.DocumentNode.SelectSingleNode("//section[contains(@class, 'article-body')]");

        if (articleBodyNode is null) return;
        
        var contentNodes = articleBodyNode.SelectNodes(".//*[not(ancestor::blockquote[@class='twitter-tweet'])]");

        if (contentNodes is null) return;
        
        foreach (HtmlNode node in contentNodes)
        {
            switch (node.Name)
            {
                case "p" when node.SelectSingleNode(".//blockquote[@class='twitter-tweet']") is null:
                {
                    StringBuilder builder = new();
                    extractTextWithFormatting(node, builder);
                    string text = builder.ToString();
                    io_Article.BodyContents.Add(new RegularText(text));
                    break;
                }
                
                case "h4":
                {
                    string text = HtmlEntity.DeEntitize(node.InnerText);
                    io_Article.BodyContents.Add(new SubHeader(text));
                    break;
                }
                
                case "ul" when node.GetAttributeValue("class", "") != "tags":
                {
                    HtmlNodeCollection listItems = node.SelectNodes(".//li[normalize-space()]");

                    if (listItems != null)
                    {
                        foreach (HtmlNode listItem in listItems)
                        {
                            string text = HtmlEntity.DeEntitize(listItem.InnerText);
                            io_Article.BodyContents.Add(new TextListItem("• " + text));
                        }
                    }

                    break;
                }
                
                case "figure":
                {
                    HtmlNode imgNode = node.SelectSingleNode(".//img");
                    HtmlNode captionNode = node.SelectSingleNode(".//figcaption");

                    if (imgNode != null)
                    {
                        string src = imgNode.GetAttributeValue("src", null);
                        string description = captionNode != null ? HtmlEntity.DeEntitize(captionNode.InnerText) : string.Empty;

                        if (!string.IsNullOrEmpty(src))
                        {
                            io_Article.BodyContents.Add(new ImageContent(src, i_NumOfImages++));

                            if (!string.IsNullOrEmpty(description))
                            {
                                io_Article.BodyContents.Add(new ImageCredit(description));
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

    private void extractTextWithFormatting(HtmlNode i_Node, StringBuilder io_Builder)
    {
        foreach (HtmlNode child in i_Node.ChildNodes)
        {
            switch (child.Name)
            {
                case "script":
                    continue;
                
                case "strong":
                    io_Builder.Append(HtmlEntity.DeEntitize(child.InnerText).Replace("\u00A0", " "));
                    break;
                
                case "br":
                    io_Builder.Append(Environment.NewLine);
                    break;
                
                default:
                {
                    if (child.HasChildNodes)
                    {
                        extractTextWithFormatting(child, io_Builder);
                    }

                    else
                    {
                        io_Builder.Append(HtmlEntity.DeEntitize(child.InnerText).Replace("\u00A0", " "));
                    }

                    break;
                }
            }
        }
    }
}