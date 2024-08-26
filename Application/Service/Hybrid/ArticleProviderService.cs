using Application.Helpers;
using Application.Service.Local;
using Application.Service.Remote;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Hybrid;

public class ArticleProviderService
{
    private static readonly object rm_CreationLock = new();
    private static ArticleProviderService? m_Instance;
    public static ArticleProviderService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ArticleProviderService();

                return m_Instance;
            }
        }
    }
    private readonly CacheService rm_CacheService = CacheService.Instance;
    private readonly OfflineContentService rm_OfflineService = OfflineContentService.Instance;
    private readonly ArticleDownloadService rm_ArticleDownload = ArticleDownloadService.Instance;
    private readonly ConnectivityManager rm_ConnectivityManager = ConnectivityManager.Instance;

    public async Task<(Article?, bool)> ProvideArticle(OuterArticle i_OuterArticle)
    {
        Article? article = null;

        var cachedArticle = rm_CacheService.FindCachedArticle(i_OuterArticle);
        var storedArticle = rm_OfflineService.FindStoredArticle(i_OuterArticle);
        bool isStored = storedArticle is not null;

        if (cachedArticle is not null)
        {
            article = cachedArticle;
        }
                
        else if (storedArticle is not null)
        {
            article = storedArticle;
        }
                
        else if (!rm_ConnectivityManager.IsDeviceConnected())
        {
            await rm_ConnectivityManager.AlertConnectivityIssue();
            return (article, isStored);
        }
                
        else
        {
            article = await rm_ArticleDownload.DownloadArticle(i_OuterArticle);
        }

        return (article, isStored);
    }
}