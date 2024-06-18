namespace OfflineReader.Model.HTMLParser;

public interface IMainPageParser
{
    List<Article> ParseHTML(string i_HTML);
}