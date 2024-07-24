namespace OfflineReader.Model.HTMLParser.MainPageParser;

public interface IMainPageParser
{
    List<Article> ParseHTML(string i_HTML);
}