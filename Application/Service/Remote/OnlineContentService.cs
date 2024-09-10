using System.Collections.ObjectModel;
using BusinessLogic.Article.Partials;
using BusinessLogic.HTMLParser.MainPageParser;
using Application.Service.Local;
using BusinessLogic.SupportedWebsite;

namespace Application.Service.Remote;

public sealed class OnlineContentService
{
    private static readonly object sr_CreationLock = new();
    private static OnlineContentService? s_Instance;
    public static OnlineContentService Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new OnlineContentService();

                return s_Instance;
            }
        }
    }
    private ConfigService ConfigService { get; } = ConfigService.Instance;
    private HTMLSupplierService HTMLSupplier { get; } = HTMLSupplierService.Instance;
    public ObservableCollection<OuterArticle> OnlineArticlesList { get; } = new();

    private OnlineContentService() {}

    public async Task UpdateArticlesList()
    {
        List<OuterArticle> articles = new List<OuterArticle>();
        List<SupportedWebsite> supportedWebsites = await ConfigService.LoadSupportedWebsites();

        foreach (SupportedWebsite supportedWebsite in supportedWebsites)
        {
            string webURL = supportedWebsite.URL;
            string htmlCode = await HTMLSupplier.GetHTMLAsync(webURL);
            IMainPageParser? mainPageParser = MainPageParserFactory.GenerateMainPageParser(supportedWebsite.Name);
            List<OuterArticle>? websiteArticles = mainPageParser?.ParseMainPageHTML(htmlCode);

            if (websiteArticles is not null) articles.AddRange(websiteArticles);
        }
        
        removeDuplicateArticles(ref articles);

        if (areArticlesDifferent(articles))
        {
            OnlineArticlesList.Clear();
            
            foreach (OuterArticle article in articles)
            {
                OnlineArticlesList.Add(article);
            }
        }
    }
    
    private void removeDuplicateArticles(ref List<OuterArticle> io_Articles)
    {
        List<OuterArticle> distinctArticles = io_Articles
            .GroupBy(article => article.Title)
            .Select(group => group.First())
            .ToList();

        io_Articles.Clear();
        io_Articles.AddRange(distinctArticles);
    }
    
    private bool areArticlesDifferent(List<OuterArticle> i_Articles)
    {
        bool areDifferent = false;

        if (OnlineArticlesList.Count == 0 || OnlineArticlesList.Count != i_Articles.Count)
        {
            areDifferent = true;
        }

        else
        {
            for (int i = 0; i < i_Articles.Count && !areDifferent; i++)
            {
                if (!isArticleInCollection(i_Articles[i]))
                {
                    areDifferent = true;
                }
            }
        }

        return areDifferent;
    }
    
    private bool isArticleInCollection(OuterArticle i_Article)
    {
        bool foundArticle = false;

        for (int i = 0; i < OnlineArticlesList.Count && !foundArticle; i++)
        {
            if (i_Article.Title.Equals(OnlineArticlesList[i].Title))
            {
                foundArticle = true;
            }
        }

        return foundArticle;
    }
}