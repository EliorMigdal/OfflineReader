using System.Text;
using OfflineReader.Model;

namespace OfflineReader.Service.Local;

public class PathGenerator
{
    private static PathGenerator m_Instance;
    public static PathGenerator Instance
    {
        get
        {
            m_Instance ??= new PathGenerator();

            return m_Instance;
        }
    }
    private ArticleIDGenerator m_ArticleIDGenerator = ArticleIDGenerator.Instance;

    private PathGenerator() { }

    public string GenerateArticlePath(Article i_Article)
    {
        StringBuilder pathBuilder = new();

        string webSiteName = i_Article.Website.ToLower();
        string articleDate = i_Article.Date.Replace("-", "");
        string articleID = m_ArticleIDGenerator.GenerateArticleID(i_Article);

        pathBuilder.Append('/').Append(webSiteName).Append('/').Append(articleDate).Append('/').Append(articleID).Append('/');

        return pathBuilder.ToString();
    }
}