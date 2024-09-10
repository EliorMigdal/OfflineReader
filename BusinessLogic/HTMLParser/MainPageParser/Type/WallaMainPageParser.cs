using System.Text.RegularExpressions;
using BusinessLogic.Article.Content;
using BusinessLogic.Article.Partials;
using HtmlAgilityPack;

namespace BusinessLogic.HTMLParser.MainPageParser.Type;

public class WallaMainPageParser : IMainPageParser
{
    private static readonly object sr_CreationLock = new();
    private static WallaMainPageParser? s_Instance;
    public static WallaMainPageParser Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new WallaMainPageParser();

                return s_Instance;
            }
        }
    }
    private string m_LogoSource = string.Empty;
    private const string k_Website = "walla";
    private const string k_BaseURL = "https://www.walla.co.il";
    
    private WallaMainPageParser() {}
    
    public List<OuterArticle> ParseMainPageHTML(string i_HTML)
    {
        HtmlDocument HTMLDocument = new HtmlDocument();
        HTMLDocument.LoadHtml(i_HTML);
        
        return generateArticles(HTMLDocument);
    }

    private List<OuterArticle> generateArticles(HtmlDocument i_HTMLDocument)
    {
        List<OuterArticle> articles = new List<OuterArticle>();
        HtmlNodeCollection articleNodes = i_HTMLDocument.DocumentNode.SelectNodes("//article");
        extractLogoURL(i_HTMLDocument);

        if (articleNodes is null) return articles;
        foreach (HtmlNode articleNode in articleNodes)
        {
            string url = extractURL(articleNode);
            string title = extractTitle(articleNode);
            DateTime date = DateTime.Today;

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title)) continue;

            OuterArticle article = new OuterArticle
            {
                Title = title,
                URL = url,
                Website = k_Website,
                Date = date,
                MainImage = new ImageContent(m_LogoSource, 0)
            };
            
            GenerateArticleID(article);
            articles.Add(article);
        }
        
        return articles;
    }
    
    private void extractLogoURL(HtmlDocument i_HTMLDocument)
    {
        var imgNode = i_HTMLDocument.DocumentNode.SelectSingleNode("//div[@class='no-mobile logo-wrapper']//img");

        if (imgNode is null) return;
        m_LogoSource = imgNode.GetAttributeValue("src", string.Empty);
        if (!string.IsNullOrEmpty(m_LogoSource)) m_LogoSource = k_BaseURL + m_LogoSource;
    }
    
    private string extractTitle(HtmlNode i_SlotView)
    {
        HtmlNode titleNode = i_SlotView.SelectSingleNode(".//h3 | .//h2");
        
        return titleNode?.InnerText.Trim().Replace("&quot;", "\"").Replace("&#x27", "\'") ?? "";
    }
    
    private string extractURL(HtmlNode articleNode)
    {
        string parentHref = string.Empty, childHref = string.Empty;
        HtmlNode childNode = articleNode.SelectSingleNode(".//a[@href and " +
                                                          "not(contains(@href, 'sport1')) and " +
                                                          "contains(@href, 'walla')]");
        HtmlNode parentNode = articleNode.ParentNode;
        
        if (parentNode is not null) parentHref = parentNode.GetAttributeValue("href", string.Empty);
        if (childNode is not null) childHref = childNode.GetAttributeValue("href", string.Empty);

        return string.IsNullOrEmpty(parentHref) ? childHref : parentHref;
    }

    public void GenerateArticleID(OuterArticle i_Article)
    {
        string regularExpression = @"\/item\/(\d+)";
        Match match = Regex.Match(i_Article.URL, regularExpression);

        i_Article.ID = match.Success ? $"{k_Website}{match.Groups[1].Value}" : i_Article.URL;
    }
}