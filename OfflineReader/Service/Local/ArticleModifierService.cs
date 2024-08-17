using System.Diagnostics;
using OfflineReader.Model;
using OfflineReader.Model.ArticleContent;
using OfflineReader.Service.Remote;

namespace OfflineReader.Service.Local;

public sealed class ArticleModifierService
{
    private static ArticleModifierService? m_Instance;
    public static ArticleModifierService Instance
    {
        get
        {
            m_Instance ??= new ArticleModifierService();

            return m_Instance;
        }
    }
    private readonly ArticleSerializerService m_ArticleSerializer = ArticleSerializerService.Instance;
    private readonly ImageDownloadService m_ImageService = ImageDownloadService.Instance;
    private readonly PathGenerator m_PathGenerator = PathGenerator.Instance;
    
    private ArticleModifierService() {}

    public async Task<Article?> SaveArticle(Article i_Article, string i_Path)
    {
        Article? merged = Article.MergeInnerAndOuterObjects(SharedData.SharedArticle, i_Article);
        string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(merged);
        
        if (!Directory.Exists(articlePath))
        {
            Directory.CreateDirectory(articlePath);
        }

        try
        {
            await saveArticleImages(merged, articlePath);
            m_ArticleSerializer.SerializeArticle(merged, articlePath);
            SharedData.ParsedArticle = merged;
        }

        catch (Exception error)
        {
            Debug.WriteLine(error);
            merged = null;
        }

        return merged;
    }

    public bool RemoveArticle(Article i_Article, string i_Path)
    {
        string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(i_Article);
        bool successfullyRemoved = true;

        if (!Directory.Exists(articlePath))
            return false;
        
        try
        {
            Directory.Delete(articlePath, true);
        }
            
        catch (Exception e)
        {
            Debug.WriteLine(e);
            successfullyRemoved = false;
        }

        return successfullyRemoved;
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

            if (i_Article.Author.Image.Length > 0)
            {
                await m_ImageService.DownloadImageAsync(i_Article.Author.Image, i_ArticlePath + "author.jpg");
                i_Article.Author.Image = i_ArticlePath + "author.jpg";
            }
            
            foreach (BodyContent bodyContent in i_Article.ArticleBody)
            {
                if (bodyContent is not ImageContent image) continue;
                filePath = i_ArticlePath + image.ImageID + ".jpg";
                await m_ImageService.DownloadImageAsync(image.Content, filePath);
                image.Content = filePath;
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