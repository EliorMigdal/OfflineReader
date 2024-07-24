using HtmlAgilityPack;
using OfflineReader.Model.ArticleContent;
using OfflineReader.Model.ArticleContent.TextType;

namespace OfflineReader.Model.HTMLParser.ArticleParser.Type;

public class MakoArticleParser : IArticleParser
{
    [Obsolete]
    public Article ParseHTML(string i_HTML)
    {
        HtmlDocument htmlDocument = new();
        Article article = new();

        htmlDocument.LoadHtml(i_HTML);
        extractArticleTitle(article, htmlDocument);
        extractArticleDescription(article, htmlDocument);
        extractArticleAuthorInfo(article, htmlDocument);
        extractArticleDatesInfo(article, htmlDocument);
        extractHeaderImage(article, htmlDocument);
        extractArticleBody(article, htmlDocument);

        return article;
    }

    private void extractArticleTitle(Article io_Article, HtmlDocument i_HTML)
    {
        HtmlNode headlineNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='headline']");

        if (headlineNode != null && headlineNode.Attributes["content"] != null)
        {
            string headline = HtmlEntity.DeEntitize(headlineNode.Attributes["content"].Value);
            io_Article.Title = headline;
        }
    }

    private void extractArticleDescription(Article io_Article, HtmlDocument i_HTML)
    {
        HtmlNode descriptionNode = i_HTML.DocumentNode.SelectSingleNode("//meta[@name='description']");

        if (descriptionNode != null)
        {
            string descriptionContent = descriptionNode.GetAttributeValue("content", string.Empty);

            if (!string.IsNullOrEmpty(descriptionContent))
            {
                string descriptionText = HtmlEntity.DeEntitize(descriptionContent);
                io_Article.Description = descriptionText;
            }
        }
    }

    private void extractArticleAuthorInfo(Article io_Article, HtmlDocument i_HTML)
    {
        HtmlNode authorInfoNode = i_HTML.DocumentNode.SelectSingleNode("//div[@class='writer-data']");

        if (authorInfoNode != null)
        {
            HtmlNode authorImageNode = authorInfoNode.SelectSingleNode(".//img[@src]");
            HtmlNode authorNameNode = authorInfoNode.SelectSingleNode(".//a[contains(@href, 'Editor-')]") ?? authorInfoNode.SelectSingleNode(".//span[@itemprop='author']");

            if (authorImageNode != null && authorNameNode != null)
            {
                string authorImageSrc = authorImageNode.GetAttributeValue("src", string.Empty);
                string authorName = HtmlEntity.DeEntitize(authorNameNode.InnerText.Trim());

                io_Article.Author.Image = authorImageSrc;
                io_Article.Author.Name = authorName;
            }
        }
    }

    private void extractArticleDatesInfo(Article io_Article, HtmlDocument i_HTML)
    {
        HtmlNode datePublishedNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='datePublished']");
        HtmlNode dateModifiedNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='dateModified']");

        if (datePublishedNode != null)
        {
            DateTime datePublished = DateTime.Parse(datePublishedNode.GetAttributeValue("content", string.Empty));
            io_Article.PublishedDate = datePublished;
        }

        if (dateModifiedNode != null)
        {
            DateTime dateModified = DateTime.Parse(dateModifiedNode.GetAttributeValue("content", string.Empty));
            io_Article.LastUpdated = dateModified;
        }
    }

    private void extractHeaderImage(Article io_Article, HtmlDocument i_HTML)
    {
        HtmlNode initialImageNode = i_HTML.DocumentNode.SelectSingleNode("//section[contains(@class, 'article-header i-pic')]/figure");

        if (initialImageNode != null)
        {
            HtmlNode imgNode = initialImageNode.SelectSingleNode(".//img");
            HtmlNode captionNode = initialImageNode.SelectSingleNode(".//figcaption");

            if (imgNode != null)
            {
                string src = imgNode.GetAttributeValue("src", null);
                string description = captionNode != null ? HtmlEntity.DeEntitize(captionNode.InnerText) : string.Empty;

                if (!string.IsNullOrEmpty(src))
                {
                    io_Article.ArticleBody.Add(new ImageContent(src));

                    if (!string.IsNullOrEmpty(description))
                    {
                        io_Article.ArticleBody.Add(new ImageCredit(description));
                    }
                }
            }
        }
    }

    private void extractArticleBody(Article io_Article, HtmlDocument i_HTML)
    {
        HtmlNode articleBodyNode = i_HTML.DocumentNode.SelectSingleNode("//section[contains(@class, 'article-body')]");

        if (articleBodyNode != null)
        {
            HtmlNodeCollection contentNodes = articleBodyNode.SelectNodes(".//p[not(.//figure)]|.//figure|.//h4|.//ul[not(@class='tags')]|.//li");

            if (contentNodes != null)
            {
                foreach (HtmlNode node in contentNodes)
                {
                    string text = HtmlEntity.DeEntitize(node.InnerText);

                    if (node.Name == "p")
                    {
                        io_Article.ArticleBody.Add(new RegularText(text));
                    }

                    else if (node.Name == "h4")
                    {
                        io_Article.ArticleBody.Add(new SubHeader(text));
                    }

                    else if (node.Name == "ul")
                    {
                        HtmlNodeCollection listItems = node.SelectNodes(".//li[normalize-space()]");

                        if (listItems != null)
                        {
                            foreach (HtmlNode listItem in listItems)
                            {
                                text = HtmlEntity.DeEntitize(listItem.InnerText);
                                io_Article.ArticleBody.Add(new TextListItem("• " + text));
                            }
                        }
                    }

                    else if (node.Name == "figure")
                    {
                        HtmlNode imgNode = node.SelectSingleNode(".//img");
                        HtmlNode captionNode = node.SelectSingleNode(".//figcaption");

                        if (imgNode != null)
                        {
                            string src = imgNode.GetAttributeValue("src", null);
                            string description = captionNode != null ? HtmlEntity.DeEntitize(captionNode.InnerText) : string.Empty;

                            if (!string.IsNullOrEmpty(src))
                            {
                                io_Article.ArticleBody.Add(new ImageContent(src));

                                if (!string.IsNullOrEmpty(description))
                                {
                                    io_Article.ArticleBody.Add(new ImageCredit(description));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}