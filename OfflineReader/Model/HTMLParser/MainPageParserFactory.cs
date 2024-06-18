using OfflineReader.Model.HTMLParser.MainPageParsers;

namespace OfflineReader.Model.HTMLParser;

public class MainPageParserFactory
{
    public IMainPageParser GenerateMainPageParser(string i_Website)
    {
        IMainPageParser parser;

        switch (i_Website)
        {
            case "mako":
                parser = new MakoMainPageParser();
                break;

            default:
                parser = null;
                break;
        }

        return parser;
    }
}