using System.Text;
using System.Text.RegularExpressions;
using BusinessLogic.Article.Content;
using BusinessLogic.Article.Partials;
using HtmlAgilityPack;

namespace BusinessLogic.HTMLParser.MainPageParser.Type;

public sealed class MakoMainPageParser : IMainPageParser
{
    private static readonly object rm_CreationLock = new();
    private const string k_BaseURL = "https://www.mako.co.il";
    private const string k_Website = "mako";
    private static MakoMainPageParser? m_Instance;
    public static MakoMainPageParser Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new MakoMainPageParser();

                return m_Instance;
            }
        }
    }
    
    private MakoMainPageParser() {}

    public List<OuterArticle> ParseMainPageHTML(string i_HTML)
    {
        List<OuterArticle> articles = new List<OuterArticle>();
        HtmlDocument HTMLDocument = new HtmlDocument();
        HTMLDocument.LoadHtml(i_HTML);
        readSpotlightArticles(ref articles, HTMLDocument);

        return articles;
    }

    private void readSpotlightArticles(ref List<OuterArticle> io_Articles, HtmlDocument i_HTML)
    {
        HtmlNodeCollection articleNodes = i_HTML.DocumentNode.SelectNodes("//article");

        if (articleNodes is null) return;
        foreach (HtmlNode articleNode in articleNodes)
        {
            try
            {
                string articleTitle = extractArticleTitle(articleNode);
                string articleURL = extractArticleURL(articleNode);
                string articleImage = extractArticleImage(articleNode);
                DateTime articleDate = extractArticleDate(articleNode);

                if (articleURL.Length <= 0 || articleTitle.Length <= 0 || articleImage.Length <= 0) continue;

                OuterArticle article = new OuterArticle
                {
                    Title = articleTitle,
                    URL = articleURL,
                    MainImage = new ImageContent(articleImage, 0),
                    Website = k_Website,
                    Date = articleDate
                };

                GenerateArticleID(article);
                io_Articles.Add(article);
            }

            catch (Exception)
            {
                // ignored
            }
        }
    }

    private string extractArticleTitle(HtmlNode i_ArticleNode)
    {
        string articleTitle = string.Empty;
        HtmlNode titleNode = i_ArticleNode.SelectSingleNode(".//h4 | .//h2");

        if (titleNode is not null)
        {
            articleTitle = HtmlEntity.DeEntitize(titleNode.InnerText);
        }

        return articleTitle;
    }

    private string extractArticleURL(HtmlNode i_ArticleNode)
    {
        string articleURL = string.Empty;
        HtmlNode URLNode = i_ArticleNode.SelectSingleNode(".//a");

        if (URLNode is null) return articleURL;
        articleURL = URLNode.GetAttributeValue("href", "");

        if (!string.IsNullOrWhiteSpace(articleURL) && !articleURL.StartsWith("http"))
        {
            articleURL = new Uri(new Uri(k_BaseURL), articleURL).AbsoluteUri;
        }

        return articleURL;
    }

    private string extractArticleImage(HtmlNode i_ArticleNode)
    {
        string articleImageURLs = string.Empty;
        HtmlNode imageNode = i_ArticleNode.SelectSingleNode(".//img");

        if (imageNode is null) return articleImageURLs;
        articleImageURLs = imageNode.GetAttributeValue("srcSet", "");

        if (!string.IsNullOrWhiteSpace(articleImageURLs))
        {
            articleImageURLs = findFirstImage(articleImageURLs);
        }

        return articleImageURLs;
    }

    private DateTime extractArticleDate(HtmlNode i_ArticleNode)
    {
        HtmlNode dateNode = i_ArticleNode.SelectSingleNode(".//time");

        if (dateNode is null ||
            !DateTime.TryParse(dateNode.GetAttributeValue("dateTime", string.Empty), out DateTime o_Date))
            throw new Exception();
        
        return o_Date;
    }

    private string findFirstImage(string i_ImageURL)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(k_BaseURL);

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
    
    public void GenerateArticleID(OuterArticle io_Article)
    {
        string regularExpression = @"Article-([a-zA-Z0-9]+)\.htm";
        Match match = Regex.Match(io_Article.URL, regularExpression);

        io_Article.ID = match.Success ? $"{k_Website}{match.Groups[1].Value}" : io_Article.URL;
    }
}