using BusinessLogic.HTMLParser.ArticleParser.Type;

namespace BusinessLogic.HTMLParser.ArticleParser;

public sealed class ArticleParserFactory
{
    private static readonly object rm_CreationLock = new();
    private readonly object rm_GenerationLock = new();
    private static ArticleParserFactory? m_Instance;
    public static ArticleParserFactory Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;
            
            lock (rm_CreationLock)
            {
                m_Instance ??= new ArticleParserFactory();

                return m_Instance;
            }
        }
    }
    
    private ArticleParserFactory() {}
    
    public IArticleParser? GenerateParser(string i_Website)
    {
        lock (rm_GenerationLock)
        {
            IArticleParser? parser = i_Website switch
            {
                "mako" => MakoArticleParser.Instance,
                _ => null,
            };

            return parser; 
        }
    }
}