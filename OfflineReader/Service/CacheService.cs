using OfflineReader.Model;
using OfflineReader.Model.ArticleContent;
using System.Diagnostics;

namespace OfflineReader.Service;

public class CacheService
{
    public static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "Cache");
    private PathParsingService m_PathParser = new PathParsingService();
    private ArticleSerializerService m_ArticleSerializer = new ArticleSerializerService();
    private ImageDownloadService m_ImageService = new ImageDownloadService();

    public Article FindCachedArticle(Article i_Article)
    {
        return searchForArticle(i_Article);
    }

    public async Task CacheArticle(Article i_Article)
    {
        Article merged = Article.MergeInnerAndOuterObjects(SharedData.SharedArticle, i_Article);
        Debug.WriteLine($"Successfully merged!");
        string articlePath = CachePath + m_PathParser.ParseArticleToPath(merged);

        if (!Directory.Exists(articlePath))
        {
            Directory.CreateDirectory(articlePath);
        }

        Debug.WriteLine($"Got path: {articlePath}!");

        await saveArticleImages(merged, articlePath);
        Debug.WriteLine($"Successfully saved images!");
        m_ArticleSerializer.SerializeArticle(merged, articlePath);
        Debug.WriteLine($"Successfully serialized!");
    }

    private Article searchForArticle(Article i_Article)
    {
        Article article = null;
        string articlePath = CachePath + m_PathParser.ParseArticleToPath(i_Article);
        Debug.WriteLine($"Searching for article in path {articlePath}");

        if (File.Exists(articlePath) && !articlePath.Equals(string.Empty))
        {
            article = m_ArticleSerializer.DeserializeArticle(articlePath);
        }

        return article;
    }

    private async Task saveArticleImages(Article i_Article, string i_ArticlePath)
    {
        Debug.WriteLine($"Downloading image {i_Article.MainImage.Content}");
        await m_ImageService.DownloadImageAsync(i_Article.MainImage.Content, i_ArticlePath + i_Article.MainImage.ImageID.ToString() + ".jpg");
        i_Article.MainImage.Content = i_ArticlePath + i_Article.MainImage.ImageID.ToString() + ".jpg";
        Debug.WriteLine($"Updated source to {i_ArticlePath + i_Article.MainImage.ImageID.ToString() + ".jpg"}");

        Debug.WriteLine($"About to through article's body, of size {i_Article.ArticleBody.Count}");
        Debug.WriteLine($"Is it merged? Inner title: {i_Article.InnerTitle}");

        foreach (BodyContent bodyContent in i_Article.ArticleBody)
        {
            if (bodyContent is ImageContent)
            {
                Debug.WriteLine($"Found type image!");
                ImageContent image = bodyContent as ImageContent;
                Debug.WriteLine($"Downloading image {image.Content}");
                await m_ImageService.DownloadImageAsync(image.Content, i_ArticlePath + image.ImageID.ToString() + ".jpg");
                image.Content = i_ArticlePath + image.ImageID.ToString();
                Debug.WriteLine($"Updated source to {i_ArticlePath + image.ImageID.ToString()}");
            }
        }
    }
}