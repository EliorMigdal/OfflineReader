using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class CacheService
{
    private static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "Cache");
    private readonly ArticleModifierService r_ArticleModifier = ArticleModifierService.Instance;
    private readonly ArticleFinderService r_ArticleFinder = ArticleFinderService.Instance;
    private static readonly object sr_CreationLock = new();
    private static CacheService? s_Instance;
    public static CacheService Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;
            
            lock (sr_CreationLock)
            {
                s_Instance ??= new CacheService();
                
                return s_Instance;
            }
        }
    }

    private CacheService() { }

    public Article? FindCachedArticle(OuterArticle i_Article)
    {
        return r_ArticleFinder.SearchForArticle(i_Article, CachePath);
    }

    public void CacheArticle(Article i_Article)
    {
        r_ArticleModifier.SaveArticle(i_Article, CachePath);
    }
}