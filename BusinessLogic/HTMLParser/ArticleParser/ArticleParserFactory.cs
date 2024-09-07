using BusinessLogic.HTMLParser.ArticleParser.Type;

namespace BusinessLogic.HTMLParser.ArticleParser;

public static class ArticleParserFactory
{
    public static IArticleParser? GenerateParser(string i_Website)
    {
        IArticleParser? parser = i_Website switch
        {
            "mako" => MakoArticleParser.Instance,
            _ => null,
        };

        return parser; 
    }
}