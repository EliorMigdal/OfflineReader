using System.Diagnostics;
using OfflineReader.Model;
using OfflineReader.Model.ArticleContent;
using OfflineReader.Service.Remote;

namespace OfflineReader.Service.Local;

public class ArticleSaverService
{
    private static ArticleSaverService m_Instance;
    public static ArticleSaverService Instance
    {
        get
        {
            m_Instance ??= new ArticleSaverService();

            return m_Instance;
        }
    }
    private readonly ArticleSerializerService m_ArticleSerializer = ArticleSerializerService.Instance;
    private readonly ImageDownloadService m_ImageService = ImageDownloadService.Instance;
    private readonly PathGenerator m_PathGenerator = PathGenerator.Instance;
    
    private ArticleSaverService() {}

    public async Task<bool> SaveArticle(Article i_Article, string i_Path)
    {
        bool savedSuccessfully = true;
        Article merged = Article.MergeInnerAndOuterObjects(SharedData.SharedArticle, i_Article);
        string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(merged);
        
        if (!Directory.Exists(articlePath))
        {
            Directory.CreateDirectory(articlePath);
        }
        
        try
        {
            await saveArticleImages(merged, articlePath);
            m_ArticleSerializer.SerializeArticle(merged, articlePath);
            Debug.WriteLine($"Serialized at {articlePath}");
        }

        catch (Exception error)
        {
            savedSuccessfully = false;
            Debug.WriteLine(error);
        }
        
        return savedSuccessfully;
    }
    
    private async Task saveArticleImages(Article i_Article, string i_ArticlePath)
    {
        if (SharedData.Cached)
        {
            copyArticleImages(i_Article, i_ArticlePath);
        }

        else
        {
            string filePath = i_ArticlePath + i_Article.MainImage.ImageID + ".jpg";
            await m_ImageService.DownloadImageAsync(i_Article.MainImage.Content, filePath);
            i_Article.MainImage.Content = filePath;
            
            foreach (BodyContent bodyContent in i_Article.ArticleBody)
            {
                if (bodyContent is ImageContent)
                {
                    ImageContent image = bodyContent as ImageContent;
                    filePath = i_ArticlePath + image.ImageID + ".jpg";
                    await m_ImageService.DownloadImageAsync(image.Content, filePath);
                    image.Content = filePath;
                }
            }
        }
    }

    private void copyArticleImages(Article i_Article, string i_ArticlePath)
    {
        copyImage(i_Article.MainImage.Content, i_ArticlePath);

        foreach (BodyContent bodyContent in i_Article.ArticleBody)
        {
            if (bodyContent is ImageContent image)
            {
                copyImage(image.Content, i_ArticlePath);
            }
        }
    }
    
    private void copyImage(string i_FileSource, string i_DestinationFolder)
    {
        string fileName = Path.GetFileName(i_FileSource);
        string destFile = Path.Combine(i_DestinationFolder, fileName);
        File.Copy(i_FileSource, destFile, overwrite: true);
    }
}