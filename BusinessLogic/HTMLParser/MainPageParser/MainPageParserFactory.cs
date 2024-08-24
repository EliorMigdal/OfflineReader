using BusinessLogic.HTMLParser.MainPageParser.Type;

namespace BusinessLogic.HTMLParser.MainPageParser;

public sealed class MainPageParserFactory
{
    private static readonly object rm_CreationLock = new();
    private readonly object rm_GenerationLock = new();
    private static MainPageParserFactory? m_Instance;
    public static MainPageParserFactory Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new MainPageParserFactory();
                
                return m_Instance;
            }
        }
    }
    
    public IMainPageParser? GenerateMainPageParser(string i_Website)
    {
        lock (rm_GenerationLock)
        {
            IMainPageParser? parser = i_Website switch
            {
                "mako" => MakoMainPageParser.Instance,
                _ => null
            };

            return parser;
        }
    }
}