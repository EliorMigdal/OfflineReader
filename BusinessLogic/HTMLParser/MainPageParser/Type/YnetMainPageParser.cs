using System.Text.RegularExpressions;
using BusinessLogic.Article.Content;
using BusinessLogic.Article.Partials;
using HtmlAgilityPack;

namespace BusinessLogic.HTMLParser.MainPageParser.Type;

public class YnetMainPageParser : IMainPageParser
{
    private static readonly object sr_CreationLock = new();
    private static YnetMainPageParser? s_Instance;
    public static YnetMainPageParser Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new YnetMainPageParser();

                return s_Instance;
            }
        }
    }
    private string m_LogoSource = string.Empty;
    private const string k_Website = "ynet";
    
    private YnetMainPageParser() {}
    
    public List<OuterArticle> ParseMainPageHTML(string i_HTML)
    {
        HtmlDocument HTMLDocument = new HtmlDocument();
        HTMLDocument.LoadHtml(i_HTML);
        
        return generateArticles(HTMLDocument);
    }

    private List<OuterArticle> generateArticles(HtmlDocument i_HTMLDocument)
    {
        List<OuterArticle> articles = new List<OuterArticle>();
        HtmlNodeCollection slotViews = i_HTMLDocument.DocumentNode.
            SelectNodes("//div[contains(@class, 'slotView')]");
        extractLogoURL(i_HTMLDocument);

        if (slotViews is null) return articles;
        
        foreach (HtmlNode slotView in slotViews)
        {
            try
            {
                string url = extractURL(slotView);
                string title = extractTitle(slotView);
                DateTime date = extractDate(slotView);
                string imageUrl = extractImageURL(slotView);
                
                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(title)) continue;
            
                OuterArticle article = new OuterArticle
                {
                    Website = k_Website,
                    Title = title.Replace("&quot;", "\"").Replace("&#x27", "\'"),
                    URL = url,
                    Date = date,
                    MainImage = new ImageContent(imageUrl, 0)
                };
                    
                GenerateArticleID(article);
                articles.Add(article);
            }
            
            catch (Exception)
            {
                //ignored
            }
        }
        
        return articles;
    }

    private void extractLogoURL(HtmlDocument i_HTMLDocument)
    {
        var logoNode = i_HTMLDocument.DocumentNode.SelectSingleNode("//div[@class='logo']//img");

        if (logoNode == null) return;
        m_LogoSource = logoNode.GetAttributeValue("src", string.Empty);
    }

    private string extractURL(HtmlNode i_SlotView)
    {
        string url = string.Empty;
        HtmlNode anchor = i_SlotView.SelectSingleNode(".//a[@id and @href and not(contains(@href, 'live')) and " +
                                                      "not(contains(@href, 'pplus')) and " +
                                                      "not(contains(@href, 'calcalist'))]");

        if (anchor is null) return url;
        url = anchor.GetAttributeValue("href", "");

        return url;
    }

    private string extractTitle(HtmlNode i_SlotView)
    {
        HtmlNode titleNode = i_SlotView.SelectSingleNode(".//div[starts-with(@class, 'slotTitle')] | " +
                                                         ".//div[starts-with(@class, 'slotTitle')]/h1 | " +
                                                         ".//div[starts-with(@class, 'slotTitle')]/h2 | " +
                                                         ".//h1[contains(@class, 'slotTitle')] | " +
                                                         ".//h2[contains(@class, 'slotTitle')]");
        
        return titleNode?.InnerText.Trim() ?? "";
    }

    private DateTime extractDate(HtmlNode i_SlotView)
    {
        HtmlNode timeNode = i_SlotView.SelectSingleNode(".//time");
        
        if (timeNode is null ||
            !DateTime.TryParse(timeNode.GetAttributeValue("dateTime", string.Empty), out DateTime o_Date))
            throw new Exception();

        return o_Date;
    }

    private string extractImageURL(HtmlNode i_SlotView)
    {
        HtmlNode imgNode = i_SlotView.SelectSingleNode(".//img[@src]");
        
        return imgNode?.GetAttributeValue("src", "") ?? m_LogoSource;
    }

    public void GenerateArticleID(OuterArticle i_Article)
    {
        string regularExpression = @"/article/([a-zA-Z0-9]+)";
        Match match = Regex.Match(i_Article.URL, regularExpression);
        
        i_Article.ID = match.Success ? $"{k_Website}{match.Groups[1].Value}" : i_Article.URL;
    }
}