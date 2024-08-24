using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class ArticleFinderService
{
    private static readonly object rm_CreationLock = new();
    private readonly object rm_SearchLock = new();
    private readonly PathGenerator m_PathGenerator = PathGenerator.Instance;
    private static ArticleFinderService? m_Instance;

    public static ArticleFinderService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;
            
            lock (rm_CreationLock)
            {
                m_Instance ??= new ArticleFinderService();

                return m_Instance;
            }
        }
    }
    
    private ArticleFinderService() {}

    public Article? SearchForArticle(OuterArticle i_Article, string i_Path)
    {
        lock (rm_SearchLock)
        {
            Article? article = null;
            string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(i_Article) + i_Article.ID + ".xml";

            if (File.Exists(articlePath) && !articlePath.Equals(string.Empty))
            {
                article = Article.LoadArticle(articlePath);
            }

            return article;
        }
    }
}