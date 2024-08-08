using OfflineReader.Model;

namespace OfflineReader.Service.Local;

public class OfflineContentService
{
    private static string OfflineContentPath => Path.Combine(FileSystem.AppDataDirectory, "OfflineContent");
    private readonly ArticleSaverService m_ArticleSaver = ArticleSaverService.Instance;
    private readonly ArticleFinderService m_ArticleFinder = ArticleFinderService.Instance;
    private static OfflineContentService m_Instance;
    public static OfflineContentService Instance
    {
        get
        {
            m_Instance ??= new OfflineContentService();

            return m_Instance;
        }
    }
    
    private OfflineContentService() {}
    
    public Article FindStoredArticle(Article i_Article)
    {
        return m_ArticleFinder.SearchForArticle(i_Article, OfflineContentPath);
    }

    public async Task StoreArticle(Article i_Article)
    {
        await m_ArticleSaver.SaveArticle(i_Article, OfflineContentPath);
    }
}