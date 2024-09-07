using System.Globalization;
using System.Text;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class PathGenerator
{
    private static readonly object sr_CreationLock = new();
    private static PathGenerator? s_Instance;
    public static PathGenerator Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new PathGenerator();

                return s_Instance;
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