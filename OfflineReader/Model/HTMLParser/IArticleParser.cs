namespace OfflineReader.Model.HTMLParser;

public interface IArticleParser
{
    StackLayout ParseHTML(string i_HTML);
}