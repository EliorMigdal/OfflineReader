using OfflineReader.Model.HTMLParser.ArticleParsers;

namespace OfflineReader.Model.HTMLParser;

public class ArticleParserFactory
{
    public ArticleParser generateParser(string i_Website)
    {
        ArticleParser parser;

        switch (i_Website)
        {
            case "mako":
                parser = new MakoArticleParser();
                break;

            default:
                parser = null;
                break;
        }

        return parser;
    }
}