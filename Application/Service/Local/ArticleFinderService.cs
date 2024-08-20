using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public class ArticleFinderService
{
    private readonly PathGenerator m_PathGenerator = PathGenerator.Instance;
    private static ArticleFinderService? m_Instance;

    public static ArticleFinderService Instance
    {
        get
        {
            m_Instance ??= new ArticleFinderService();

            return m_Instance;
        }
    }
    
    private ArticleFinderService() {}

    public Article? SearchForArticle(OuterArticle i_Article, string i_Path)
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