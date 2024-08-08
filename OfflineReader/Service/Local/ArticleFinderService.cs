using System.Diagnostics;
using OfflineReader.Model;

namespace OfflineReader.Service.Local;

public class ArticleFinderService
{
    private readonly ArticleIDGenerator m_ArticleIDGenerator = ArticleIDGenerator.Instance;
    private readonly PathGenerator m_PathGenerator = PathGenerator.Instance;
    private readonly ArticleSerializerService m_ArticleSerializer = ArticleSerializerService.Instance;
    private static ArticleFinderService m_Instance;

    public static ArticleFinderService Instance
    {
        get
        {
            m_Instance ??= new ArticleFinderService();

            return m_Instance;
        }
    }
    
    private ArticleFinderService() {}

    public Article SearchForArticle(Article i_Article, string i_Path)
    {
        Article article = null;
        string articleID = m_ArticleIDGenerator.GenerateArticleID(i_Article);
        string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(i_Article) + articleID + ".xml";

        if (File.Exists(articlePath) && !articlePath.Equals(string.Empty))
        {
            article = m_ArticleSerializer.DeserializeArticle(articlePath);
        }

        return article;
    }
}