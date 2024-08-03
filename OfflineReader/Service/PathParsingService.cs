using System.Text;
using OfflineReader.Model;

namespace OfflineReader.Service;

public class PathParsingService
{
    public string ParseArticleToPath(Article i_Article)
    {
        StringBuilder pathBuilder = new();

        string webSiteName = i_Article.Website.ToLower();
        string articleDate = i_Article.Date.Replace("-", "");
        string articleID = i_Article.ID;

        pathBuilder.Append("/").Append(webSiteName).Append("/").Append(articleDate).Append("/").Append(articleID).Append("/");

        return pathBuilder.ToString();
    }
}