using BusinessLogic.HTMLParser.MainPageParser.Type;

namespace BusinessLogic.HTMLParser.MainPageParser;

public class MainPageParserFactory
{
    private static MainPageParserFactory? m_Instance;
    public static MainPageParserFactory Instance
    {
        get
        {
            m_Instance ??= new MainPageParserFactory();

            return m_Instance;
        }
    }
    
    public IMainPageParser? GenerateMainPageParser(string i_Website)
    {
        IMainPageParser? parser = i_Website switch
        {
            "mako" => MakoMainPageParser.Instance,
            _ => null
        };

        return parser;
    }
}