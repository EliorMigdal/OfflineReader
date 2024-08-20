using BusinessLogic.Article.Partials;

namespace BusinessLogic.HTMLParser.MainPageParser;

public interface IMainPageParser
{
    List<OuterArticle> ParseMainPageHTML(string i_HTML);
}