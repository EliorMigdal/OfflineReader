namespace OfflineReader.Model.HTMLParser.ArticleParser;

public interface IArticleParser
{
    Article ParseHTML(string i_HTML);
}