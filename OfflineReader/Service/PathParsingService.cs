namespace OfflineReader.Service;

public class PathParsingService
{
    public string ParseArticleToPath(Article i_Article)
    {
        StringBuilder pathBuilder = new();

        string articleDate = i_Article.Date.Replace("/", "");
        string webSiteName = i_Article.Website.ToLower();
        string articleID = i_Article.ID;

        pathBuilder.Append(articleDate).Append("/").Append(webSiteName).Append("/").Append(articleID).Append(".html");

        return pathBuilder.ToString();
    }
}