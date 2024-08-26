using Application.Service.Local;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;
using BusinessLogic.HTMLParser.ArticleParser;

namespace Application.Service.Remote;

public class ArticleDownloadService
{
    private static readonly object rm_CreationLock = new();
    private static ArticleDownloadService? m_Instance;
    public static ArticleDownloadService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ArticleDownloadService();

                return m_Instance;
            }
        }
    }
    private readonly HTMLSupplierService rm_HTMLService = HTMLSupplierService.Instance;
    private readonly ArticleParserFactory rm_ParserFactory = ArticleParserFactory.Instance;
    private readonly CacheService rm_CacheService = CacheService.Instance;
    
    private ArticleDownloadService() {}

    public async Task<Article?> DownloadArticle(OuterArticle i_OuterArticle)
    {
        Article? article = null;
        string html = await rm_HTMLService.GetHTMLAsync(i_OuterArticle.URL);
        IArticleParser? parser = rm_ParserFactory.GenerateParser(i_OuterArticle.Website);

        if (parser is null) return article;
        InnerArticle innerArticle = parser.ParseArticleHTML(html);
        article = new Article(innerArticle, i_OuterArticle);
        _ = Task.Run(() => rm_CacheService.CacheArticle(article));

        return article;
    }
}