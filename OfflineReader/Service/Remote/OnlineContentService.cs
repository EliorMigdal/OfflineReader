using System.Collections.ObjectModel;
using OfflineReader.Model;
using OfflineReader.Model.HTMLParser.MainPageParser;
using OfflineReader.Service.Local;

namespace OfflineReader.Service.Remote;

public class OnlineContentService
{
    private static OnlineContentService? m_Instance;
    public static OnlineContentService Instance
    {
        get
        {
            m_Instance ??= new OnlineContentService();

            return m_Instance;
        }
    }
    private ConfigService ConfigService { get; } = ConfigService.Instance;
    private HTMLSupplierService HTMLSupplier { get; } = HTMLSupplierService.Instance;
    private MainPageParserFactory MainPageParserFactory { get; } = new();
    public ObservableCollection<Article> OnlineArticlesList { get; } = new();

    public async Task UpdateArticlesList()
    {
        List<Article> articles = new List<Article>();
        List<string>? selectedURLs = ConfigService.LoadSupportedWebsites();

        if (selectedURLs is null) return;
        
        foreach (string webURL in selectedURLs)
        {
            string htmlCode = await HTMLSupplier.GetHTMLAsync(webURL);
            IMainPageParser mainPageParser = MainPageParserFactory.GenerateMainPageParser
                (SharedData.Pairs.FirstOrDefault(x => x.Value == webURL).Key);
            List<Article> websiteArticles = mainPageParser.ParseHTML(htmlCode);

            articles.AddRange(websiteArticles);
        }
        
        removeDuplicateArticles(ref articles);

        if (areArticlesDifferent(articles))
        {
            OnlineArticlesList.Clear();
            
            foreach (Article article in articles)
            {
                OnlineArticlesList.Add(article);
            }
        }
    }
    
    private void removeDuplicateArticles(ref List<Article> io_Articles)
    {
        List<Article> distinctArticles = io_Articles
            .GroupBy(article => article.OuterTitle)
            .Select(group => group.First())
            .ToList();

        io_Articles.Clear();
        io_Articles.AddRange(distinctArticles);
    }
    
    private bool areArticlesDifferent(List<Article> i_Articles)
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
    
    private bool isArticleInCollection(Article i_Article)
    {
        bool foundArticle = false;

        for (int i = 0; i < OnlineArticlesList.Count && !foundArticle; i++)
        {
            if (i_Article.OuterTitle.Equals(OnlineArticlesList[i].OuterTitle))
            {
                foundArticle = true;
            }
        }

        return foundArticle;
    }
}