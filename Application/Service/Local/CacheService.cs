using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class CacheService
{
    private static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "Cache");
    private readonly ArticleModifierService m_ArticleModifier = ArticleModifierService.Instance;
    private readonly ArticleFinderService m_ArticleFinder = ArticleFinderService.Instance;
    private static readonly object rm_CreationLock = new();
    private static CacheService? m_Instance;
    public static CacheService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;
            
            lock (rm_CreationLock)
            {
                m_Instance ??= new CacheService();
                
                return m_Instance;
            }
        }
    }

    private CacheService() { }

    public Article? FindCachedArticle(OuterArticle i_Article)
    {
        return m_ArticleFinder.SearchForArticle(i_Article, CachePath);
    }

    public void CacheArticle(Article i_Article)
    {
        m_ArticleModifier.SaveArticle(i_Article, CachePath);
    }
}