namespace OfflineReader.Model;

public static class SharedData
{
    public static Article? SharedArticle { get; set; }
    public static Article? ParsedArticle { get; set; }
    public static string HTML { get; set; } = string.Empty;
    public static Dictionary<string, string> Pairs { get; } = new()
    {
        {"mako", "https://www.mako.co.il" }
    };
    public static bool Cached { get; set; }
    public static bool Stored { get; set; }
}