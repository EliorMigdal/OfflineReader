using BusinessLogic.HTMLParser.ArticleParser.Type;

namespace BusinessLogic.HTMLParser.ArticleParser;

public class ArticleParserFactory
{
    private static ArticleParserFactory? m_Instance;
    public static ArticleParserFactory Instance
    {
        get
        {
            m_Instance ??= new ArticleParserFactory();

            return m_Instance;
        }
    }
    
    private ArticleParserFactory() {}
    
    public IArticleParser? GenerateParser(string i_Website)
    {
        IArticleParser? parser = i_Website switch
        {
            "mako" => MakoArticleParser.Instance,
            _ => null,
        };

        return parser;
    }
}