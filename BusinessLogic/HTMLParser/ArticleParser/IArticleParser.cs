using BusinessLogic.Article.Partials;

namespace BusinessLogic.HTMLParser.ArticleParser;

public interface IArticleParser
{
    InnerArticle ParseArticleHTML(string i_HTML);
}