using System.Diagnostics;
using BusinessLogic.Article;
using Application.Helpers;
using BusinessLogic.Article.Content;
using Application.Service.Remote;
using BusinessLogic.Article.Partials;

namespace Application.Service.Local;

public sealed class ArticleModifierService
{
    private static readonly object rm_CreationLock = new();
    private static ArticleModifierService? m_Instance;
    public static ArticleModifierService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ArticleModifierService();

                return m_Instance;
            }
        }
    }
    private readonly ImageDownloadService m_ImageService = ImageDownloadService.Instance;
    private readonly PathGenerator m_PathGenerator = PathGenerator.Instance;
    
    private ArticleModifierService() {}

    public async Task<Article?> SaveArticle(InnerArticle? i_Article, string i_Path)
    {
        Debug.Assert(SharedData.OuterArticle != null, "SharedData.OuterArticle != null");
        Debug.Assert(i_Article != null, nameof(i_Article) + " != null");
        Article? wholeArticle = new Article(i_Article, SharedData.OuterArticle);
        string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(wholeArticle.OuterArticle);
        
        if (!Directory.Exists(articlePath))
        {
            Directory.CreateDirectory(articlePath);
        }

        try
        {
            await saveArticleImages(wholeArticle, articlePath);
            wholeArticle.SaveArticle(articlePath);
            SharedData.InnerArticle = wholeArticle.InnerArticle;
            SharedData.WholeArticle = wholeArticle;
        }

        catch (Exception error)
        {
            Debug.WriteLine(error);
            wholeArticle = null;
        }

        return wholeArticle;
    }

    public bool RemoveArticle(Article i_Article, string i_Path)
    {
        string articlePath = i_Path + m_PathGenerator.GenerateArticlePath(i_Article.OuterArticle);
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
        if (SharedData.IsCurrentArticleCached)
        {
            copyArticleImages(i_Article, i_ArticlePath);
        }

        else
        {
            string filePath = i_ArticlePath + i_Article.OuterArticle.MainImage.ImageID + ".jpg";
            await m_ImageService.DownloadImageAsync(i_Article.OuterArticle.MainImage.Content, filePath);
            i_Article.OuterArticle.MainImage.Content = filePath;

            if (i_Article.InnerArticle.Author.ImageSource.Length > 0)
            {
                await m_ImageService.DownloadImageAsync(i_Article.InnerArticle.Author.ImageSource,
                    i_ArticlePath + "author.jpg");
                i_Article.InnerArticle.Author.ImageSource = i_ArticlePath + "author.jpg";
            }
            
            foreach (BodyContent bodyContent in i_Article.InnerArticle.BodyContents)
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
        copyImage(i_Article.OuterArticle.MainImage.Content, i_ArticlePath);

        foreach (BodyContent bodyContent in i_Article.InnerArticle.BodyContents)
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