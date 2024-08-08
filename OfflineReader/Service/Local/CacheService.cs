using OfflineReader.Model;

namespace OfflineReader.Service.Local;

public class CacheService
{
    private static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "Cache");
    private readonly ArticleSaverService m_ArticleSaver = ArticleSaverService.Instance;
    private readonly ArticleFinderService m_ArticleFinder = ArticleFinderService.Instance;
    private static CacheService m_Instance;
    public static CacheService Instance
    {
        get
        {
            m_Instance ??= new CacheService();

            return m_Instance;
        }
    }

    private CacheService() { }

    public Article FindCachedArticle(Article i_Article)
    {
        return m_ArticleFinder.SearchForArticle(i_Article, CachePath);
    }

    public async Task CacheArticle(Article i_Article)
    {
        await m_ArticleSaver.SaveArticle(i_Article, CachePath);
        SharedData.Cached = true;
    }
}