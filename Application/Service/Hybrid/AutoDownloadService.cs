using Application.Service.Local;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;
using BusinessLogic.HTMLParser.MainPageParser;
using BusinessLogic.SupportedWebsite;

namespace Application.Service.Hybrid;

public class AutoDownloadService
{
    private static readonly object sr_CreationLock = new();
    private static AutoDownloadService? s_Instance;
    public static AutoDownloadService Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new AutoDownloadService();

                return s_Instance;
            }
        }
    }
    private readonly ArticleProviderService r_ArticleProvider = ArticleProviderService.Instance;
    private readonly OfflineContentService r_OfflineService = OfflineContentService.Instance;
    
    private AutoDownloadService() {}

    public async Task AutoDownloadArticles(List<SupportedWebsite> i_Websites, int i_ArticlesPerSite)
    {
        using HttpClient client = new HttpClient();
        
        foreach (SupportedWebsite supportedWebsite in i_Websites)
        {
            IMainPageParser? parser = MainPageParserFactory.GenerateMainPageParser(supportedWebsite.Name);
            if (parser is null) continue;
            string HTML = await client.GetStringAsync(supportedWebsite.URL);
            List<OuterArticle> parsedList = parser.ParseMainPageHTML(HTML);

            for (int i = 0; i < i_ArticlesPerSite; i++)
            {
                (Article? article, bool isStored) = await r_ArticleProvider.ProvideArticle(parsedList[i]);
                if (isStored || article is null) continue;
                r_OfflineService.StoreArticle(article);
            }
        }
    }
}