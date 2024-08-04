using System.Text;
using HtmlAgilityPack;
using OfflineReader.Model.ArticleContent;

namespace OfflineReader.Model.HTMLParser.MainPageParser.Type;

public class MakoMainPageParser : IMainPageParser
{
    private readonly string k_BaseURL = "https://www.mako.co.il";
    private readonly string k_Website = "mako";

    public List<Article> ParseHTML(string i_HTML)
    {
        List<Article> articles = new List<Article>();
        HtmlDocument HTMLDocument = new HtmlDocument();
        HTMLDocument.LoadHtml(i_HTML);
        readSpotlightArticles(ref articles, HTMLDocument);

        return articles;
    }

    private void readSpotlightArticles(ref List<Article> io_Articles, HtmlDocument i_HTML)
    {
        HtmlNode spotlightNode = i_HTML.DocumentNode.SelectSingleNode("//div[contains(@class, 'Spotlight_root')]");

        if (spotlightNode != null)
        {
            HtmlNodeCollection articleNodes = i_HTML.DocumentNode.SelectNodes(".//article");

            foreach (HtmlNode articleNode in articleNodes)
            {
                string articleTitle = extractArticleTitle(articleNode);
                string articleURL = extractArticleURL(articleNode);
                string articleImage = extractArticleImage(articleNode);
                string articleDate = extractArticleDate(articleNode);

                if (articleURL.Length <= 0 || articleTitle.Length <= 0 || articleDate.Length <= 0 || articleImage.Length <= 0)
                {
                    continue;
                }

                io_Articles.Add(new Article
                {
                    OuterTitle = articleTitle,
                    URL = articleURL,
                    MainImage = new ImageContent(articleImage, 0),
                    Website = k_Website,
                    Date = articleDate[..10],
                    LastUpdated = DateTime.Parse(articleNode.SelectSingleNode(".//time").GetAttributeValue("dateTime", string.Empty))
                });
            }
        }
    }

    private string extractArticleTitle(HtmlNode i_ArticleNode)
    {
        string articleTitle = string.Empty;
        HtmlNode titleNode = i_ArticleNode.SelectSingleNode(".//h4 | .//h2");

        if (titleNode != null)
        {
            articleTitle = HtmlEntity.DeEntitize(titleNode.InnerText);
        }

        return articleTitle;
    }

    private string extractArticleURL(HtmlNode i_ArticleNode)
    {
        string articleURL = string.Empty;
        HtmlNode URLNode = i_ArticleNode.SelectSingleNode(".//a");

        if (URLNode != null)
        {
            articleURL = URLNode.GetAttributeValue("href", "");

            if (!string.IsNullOrWhiteSpace(articleURL) && !articleURL.StartsWith("http"))
            {
                articleURL = new Uri(new Uri(k_BaseURL), articleURL).AbsoluteUri;
            }
        }

        return articleURL;
    }

    private string extractArticleImage(HtmlNode i_ArticleNode)
    {
        string articleImageURLs = string.Empty;
        HtmlNode imageNode = i_ArticleNode.SelectSingleNode(".//img");

        if (imageNode != null)
        {
            articleImageURLs = imageNode.GetAttributeValue("srcSet", "");

            if (!string.IsNullOrWhiteSpace(articleImageURLs))
            {
                articleImageURLs = findFirstImage(articleImageURLs);
            }
        }

        return articleImageURLs;
    }

    private string extractArticleDate(HtmlNode i_ArticleNode)
    {
        string articleDate = string.Empty;
        HtmlNode dateNode = i_ArticleNode.SelectSingleNode(".//time");

        if (dateNode != null)
        {
            articleDate = dateNode.GetAttributeValue("dateTime", "");

            if (articleDate.Length > 0)
            {
                articleDate = articleDate[..10];
            }
        }

        return articleDate;
    }

    private string findFirstImage(string i_ImageURL)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("https://www.mako.co.il");

        foreach (char c in i_ImageURL)
        {
            if (!c.Equals(','))
            {
                builder.Append(c);
            }

            else
            {
                break;
            }
        }

        return builder.ToString()[..(builder.Length - 3)].Replace("amp;", string.Empty);
    }
}