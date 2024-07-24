using HtmlAgilityPack;

namespace OfflineReader.Model.HTMLParser.MainPageParser.Type;

public class MakoMainPageParser : IMainPageParser
{
    public List<Article> ParseHTML(string i_HTML)
    {
        List<Article> articles = new List<Article>();
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(i_HTML);
        HtmlNode mainContainer = doc.DocumentNode.SelectSingleNode("//div[@class='mako_scroller_container']");
        string today = DateTime.Today.ToString("dd/MM/yyyy");

        if (mainContainer != null)
        {
            HtmlNodeCollection itemNodes = mainContainer.SelectNodes(".//ul[@class='teasers']/li");

            if (itemNodes != null)
            {
                foreach (HtmlNode itemNode in itemNodes)
                {
                    HtmlNode titleNode = itemNode.SelectSingleNode(".//span[@class='headline']");
                    HtmlNode urlNode = itemNode.SelectSingleNode(".//a");
                    HtmlNode imageNode = itemNode.SelectSingleNode(".//img");

                    if (titleNode != null && urlNode != null && imageNode != null)
                    {
                        string title = titleNode.InnerText.Trim();
                        string url = urlNode.GetAttributeValue("href", "");
                        string imageUrl = imageNode.GetAttributeValue("src", "");

                        if (!string.IsNullOrWhiteSpace(url) && !url.StartsWith("http"))
                        {
                            url = new Uri(new Uri("https://www.mako.co.il/"), url).AbsoluteUri;
                        }

                        articles.Add(new Article
                        {
                            Title = title,
                            URL = url,
                            Image = imageUrl,
                            Website = "mako",
                            Date = today
                        });
                    }
                }
            }
        }

        return articles;
    }
}