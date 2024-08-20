using Application.Helpers;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public class CacheService
{
    private static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "Cache");
    private readonly ArticleModifierService m_ArticleModifier = ArticleModifierService.Instance;
    private readonly ArticleFinderService m_ArticleFinder = ArticleFinderService.Instance;
    private static CacheService? m_Instance;
    public static CacheService Instance
    {
        get
        {
            m_Instance ??= new CacheService();

            return m_Instance;
        }
    }

    private CacheService() { }

    public Article? FindCachedArticle(OuterArticle i_Article)
    {
        return m_ArticleFinder.SearchForArticle(i_Article, CachePath);
    }

    public async Task CacheArticle(InnerArticle? i_Article)
    {
        await m_ArticleModifier.SaveArticle(i_Article, CachePath);
        SharedData.IsCurrentArticleCached = true;
    }
}