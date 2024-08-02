namespace OfflineReader.Model;

public static class SharedData
{
    public static Article SharedArticle { get; set; }
    public static string HTML { get; set; }
    public static Dictionary<string, string> Pairs { get; set; } = new Dictionary<string, string>
    {
        {"mako", "https://www.mako.co.il" }
    };
}