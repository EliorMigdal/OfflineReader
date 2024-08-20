using System.Text;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public class PathGenerator
{
    private static PathGenerator? m_Instance;
    public static PathGenerator Instance
    {
        get
        {
            m_Instance ??= new PathGenerator();

            return m_Instance;
        }
    }

    private PathGenerator() { }

    public string GenerateArticlePath(OuterArticle i_Article)
    {
        StringBuilder pathBuilder = new();

        string webSiteName = i_Article.Website.ToLower();
        string articleDate = i_Article.Date.Replace("-", "");
        string articleID = i_Article.ID;

        pathBuilder.Append('/').Append(webSiteName).Append('/').Append(articleDate).Append('/').Append(articleID).Append('/');

        return pathBuilder.ToString();
    }
}