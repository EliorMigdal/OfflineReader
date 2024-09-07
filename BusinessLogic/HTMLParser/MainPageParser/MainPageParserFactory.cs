using BusinessLogic.HTMLParser.MainPageParser.Type;

namespace BusinessLogic.HTMLParser.MainPageParser;

public static class MainPageParserFactory
{
    public static IMainPageParser? GenerateMainPageParser(string i_Website)
    {
        IMainPageParser? parser = i_Website switch
        {
            "mako" => MakoMainPageParser.Instance,
            _ => null
        };

        return parser;
    }
}