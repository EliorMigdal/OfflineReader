using System.Globalization;
using System.Text;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class PathGenerator
{
    private static readonly object rm_CreationLock = new();
    private static PathGenerator? m_Instance;
    public static PathGenerator Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new PathGenerator();

                return m_Instance;
            }
        }
    }

    private PathGenerator() { }

    public string GenerateArticlePath(OuterArticle i_Article)
    {
        StringBuilder pathBuilder = new();

        string webSiteName = i_Article.Website.ToLower();
        string articleDate = i_Article.Date.ToString(CultureInfo.CurrentCulture).Replace("-", "");
        string articleID = i_Article.ID;

        pathBuilder.Append('/').Append(webSiteName).Append('/').Append(articleDate).Append('/').Append(articleID).Append('/');

        return pathBuilder.ToString();
    }
}