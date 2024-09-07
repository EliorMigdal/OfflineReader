using Application.Service.Local;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;
using BusinessLogic.HTMLParser.ArticleParser;

namespace Application.Service.Remote;

public class ArticleDownloadService
{
    private static readonly object sr_CreationLock = new();
    private static ArticleDownloadService? s_Instance;
    public static ArticleDownloadService Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new ArticleDownloadService();

                return s_Instance;
            }
        }
    }
    private readonly HTMLSupplierService r_HTMLService = HTMLSupplierService.Instance;
    private readonly CacheService r_CacheService = CacheService.Instance;
    
    private ArticleDownloadService() {}

    public async Task<Article?> DownloadArticle(OuterArticle i_OuterArticle)
    {
        Article? article = null;
        string html = await r_HTMLService.GetHTMLAsync(i_OuterArticle.URL);
        IArticleParser? parser = ArticleParserFactory.GenerateParser(i_OuterArticle.Website);

        if (parser is null) return article;
        InnerArticle innerArticle = parser.ParseArticleHTML(html);
        article = new Article(innerArticle, i_OuterArticle);
        _ = Task.Run(() => r_CacheService.CacheArticle(article));

        return article;
    }
}