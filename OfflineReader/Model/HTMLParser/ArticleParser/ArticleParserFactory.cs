using OfflineReader.Model.HTMLParser.ArticleParser.Type;

namespace OfflineReader.Model.HTMLParser.ArticleParser;

public class ArticleParserFactory
{
    public IArticleParser GenerateParser(string i_Website)
    {
        IArticleParser parser = i_Website switch
        {
            "mako" => new MakoArticleParser(),
            _ => null,
        };

        return parser;
    }
}