using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Helpers;

public static class SharedData
{
    public static OuterArticle? OuterArticle { get; set; }
    public static InnerArticle? InnerArticle { get; set; }

    public static Article? WholeArticle { get; set; }
    public static string HTML { get; set; } = string.Empty;
    public static Dictionary<string, string> Pairs { get; } = new()
    {
        {"mako", "https://www.mako.co.il" }
    };
    public static bool IsCurrentArticleCached { get; set; }
    public static bool IsCurrentArticleStored { get; set; }
}