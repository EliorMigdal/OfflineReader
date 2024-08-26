using System.Collections.ObjectModel;
using System.Diagnostics;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class OfflineContentService
{
    private static readonly object rm_CreationLock = new();
    private static OfflineContentService? m_Instance;
    public static OfflineContentService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new OfflineContentService();

                return m_Instance;
            }
        }
    }
    private static string OfflineContentPath => Path.Combine(FileSystem.AppDataDirectory, "OfflineContent");
    private readonly ArticleModifierService m_ArticleModifier = ArticleModifierService.Instance;
    private readonly ArticleFinderService m_ArticleFinder = ArticleFinderService.Instance;
    private ObservableCollection<Article> LocallyStoredArticles { get; } = new();
    public ObservableCollection<OuterArticle> LocallyStoredOuterArticles { get; } = new();

    private OfflineContentService() {}
    
    public Article? FindStoredArticle(OuterArticle i_Article)
    {
        return m_ArticleFinder.SearchForArticle(i_Article, OfflineContentPath);
    }

    public bool StoreArticle(Article i_Article)
    {
        bool successfullyStored = false;
        
        try
        {
            m_ArticleModifier.SaveArticle(i_Article, OfflineContentPath);
            LocallyStoredArticles.Add(i_Article);
            LocallyStoredOuterArticles.Add(i_Article.OuterArticle);
            successfullyStored = true;
        }
        
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return successfullyStored;
    }

    public bool RemoveArticle(Article i_Article)
    {
        foreach (Article article in LocallyStoredArticles)
        {
            if (!article.OuterArticle.Title.Equals(i_Article.OuterArticle.Title))
                continue;

            LocallyStoredOuterArticles.Remove(article.OuterArticle);
            LocallyStoredArticles.Remove(article);
            break;
        }
        
        return m_ArticleModifier.RemoveArticle(i_Article, OfflineContentPath);
    }

    public void InitializeOfflineArticles()
    {
        if (!Directory.Exists(OfflineContentPath)) return;
        string[] xmlFiles = Directory.GetFiles(OfflineContentPath, "*.xml", SearchOption.AllDirectories);

        foreach (string file in xmlFiles)
        {
            Article? article = Article.LoadArticle(file);

            if (article is null) continue;
            LocallyStoredArticles.Add(article);
            LocallyStoredOuterArticles.Add(article.OuterArticle);
        }
    }
}