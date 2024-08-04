using OfflineReader.Model;
using OfflineReader.Model.ArticleContent;
using OfflineReader.Service.Remote;
using System.Diagnostics;

namespace OfflineReader.Service.Local;

public class CacheService
{
    private static CacheService m_Instance;
    public static CacheService Instance
    {
        get
        {
            m_Instance ??= new CacheService();

            return m_Instance;
        }
    }

    public static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "Cache");
    private PathGenerator m_PathGenerator = PathGenerator.Instance;
    private ArticleSerializerService m_ArticleSerializer = ArticleSerializerService.Instance;
    private ImageDownloadService m_ImageService = ImageDownloadService.Instance;
    private ArticleIDGenerator m_ArticleIDGenerator = ArticleIDGenerator.Instance;

    private CacheService() { }

    public Article FindCachedArticle(Article i_Article)
    {
        return searchForArticle(i_Article);
    }

    public async Task CacheArticle(Article i_Article)
    {
        Article merged = Article.MergeInnerAndOuterObjects(SharedData.SharedArticle, i_Article);
        string articlePath = CachePath + m_PathGenerator.GenerateArticlePath(merged);

        if (!Directory.Exists(articlePath))
        {
            Directory.CreateDirectory(articlePath);
        }

        await saveArticleImages(merged, articlePath);
        m_ArticleSerializer.SerializeArticle(merged, articlePath);
    }

    private Article searchForArticle(Article i_Article)
    {
        Article article = null;
        string articleID = m_ArticleIDGenerator.GenerateArticleID(i_Article);
        string articlePath = CachePath + m_PathGenerator.GenerateArticlePath(i_Article) + articleID + ".xml";
        Debug.WriteLine($"Searching in path {articlePath}");

        if (File.Exists(articlePath) && !articlePath.Equals(string.Empty))
        {
            article = m_ArticleSerializer.DeserializeArticle(articlePath);
        }

        return article;
    }

    private async Task saveArticleImages(Article i_Article, string i_ArticlePath)
    {
        await m_ImageService.DownloadImageAsync(i_Article.MainImage.Content, i_ArticlePath + i_Article.MainImage.ImageID.ToString() + ".jpg");
        i_Article.MainImage.Content = i_ArticlePath + i_Article.MainImage.ImageID.ToString() + ".jpg";

        foreach (BodyContent bodyContent in i_Article.ArticleBody)
        {
            if (bodyContent is ImageContent)
            {
                ImageContent image = bodyContent as ImageContent;
                await m_ImageService.DownloadImageAsync(image.Content, i_ArticlePath + image.ImageID.ToString() + ".jpg");
                image.Content = i_ArticlePath + image.ImageID.ToString() + ".jpg";
            }
        }
    }
}