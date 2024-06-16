namespace OfflineReader.Model.HTMLParser;

public interface ArticleParser
{
    StackLayout ParseHTML(string i_HTML);
}