using System.Collections.ObjectModel;
using OfflineReader.Model;

namespace OfflineReader.Service.Local;

public class OfflineContentService
{
    private static OfflineContentService? m_Instance;
    public static OfflineContentService Instance
    {
        get
        {
            m_Instance ??= new OfflineContentService();

            return m_Instance;
        }
    }
    private static string OfflineContentPath => Path.Combine(FileSystem.AppDataDirectory, "OfflineContent");
    private readonly ArticleModifierService m_ArticleModifier = ArticleModifierService.Instance;
    private readonly ArticleSerializerService m_ArticleSerializer = ArticleSerializerService.Instance;
    private readonly ArticleFinderService m_ArticleFinder = ArticleFinderService.Instance;
    public ObservableCollection<Article> LocallyStoredArticles { get; } = new();

    private OfflineContentService()
    {
        Task.Run(initializeOfflineArticles);
    }
    
    public Article? FindStoredArticle(Article i_Article)
    {
        return m_ArticleFinder.SearchForArticle(i_Article, OfflineContentPath);
    }

    public async Task<bool> StoreArticle(Article i_Article)
    {
        Article? stored = await m_ArticleModifier.SaveArticle(i_Article, OfflineContentPath);
        
        if (stored is not null)
            LocallyStoredArticles.Add(stored);

        return stored is not null;
    }

    public bool RemoveArticle(Article i_Article)
    {
        foreach (Article article in LocallyStoredArticles)
        {
            if (!article.OuterTitle.Equals(i_Article.OuterTitle))
                continue;
            
            LocallyStoredArticles.Remove(article);
            break;
        }
        
        return m_ArticleModifier.RemoveArticle(i_Article, OfflineContentPath);
    }

    private void initializeOfflineArticles()
    {
        string[] xmlFiles = Directory.GetFiles(OfflineContentPath, "*.xml", SearchOption.AllDirectories);

        foreach (string file in xmlFiles)
        {
            Article? article = m_ArticleSerializer.DeserializeArticle(file);
            
            if (article is not null)
                LocallyStoredArticles.Add(article);
        }
    }
}