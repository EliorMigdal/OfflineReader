namespace OfflineReader.Model;

public class SupportedWebsite
{
    public string Name { get; private set; } = "ynet";
    public bool Selection { get; set; } = false;
    public string Logo { get; private set; } = "ynetlogo";
    public string URL { get; private set; }

    public SupportedWebsite() { }

    public SupportedWebsite(string i_Website)
    {
        Name = i_Website;
    }

    public SupportedWebsite(string i_Website, string i_URL)
    {
        Name = i_Website;
        URL = i_URL;
    }
}