using OfflineReader.Model.HTMLParser.MainPageParser.Type;

namespace OfflineReader.Model.HTMLParser.MainPageParser;

public class MainPageParserFactory
{
    public IMainPageParser GenerateMainPageParser(string i_Website)
    {
        IMainPageParser parser = i_Website switch
        {
            "mako" => new MakoMainPageParser(),
            _ => null,
        };

        return parser;
    }
}