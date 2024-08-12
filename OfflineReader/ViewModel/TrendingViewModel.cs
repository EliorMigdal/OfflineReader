using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OfflineReader.Service.Local;
using OfflineReader.Service.Remote;
using OfflineReader.Model;
using OfflineReader.Model.HTMLParser.MainPageParser;
using System.Diagnostics;
using AsyncAwaitBestPractices.MVVM;
using OfflineReader.View;

namespace OfflineReader.ViewModel;

public partial class TrendingViewModel : BaseViewModel
{
    public ICommand OnlineClickedCommand { get; private set; } = null!;
    public ICommand OfflineClickedCommand { get; private set; } = null!;
    public ICommand ArticleSelectedCommand { get; private set; } = null!;
    public ICommand LoadMoreArticlesCommand { get; private set; } = null!;

    private HTMLSupplierService HTMLSupplier { get; } = HTMLSupplierService.Instance;
    private CacheService CacheService { get; } = CacheService.Instance;
    private OfflineContentService OfflineService { get; } = OfflineContentService.Instance;
    private readonly IConnectivity m_Connectivity;

    public ObservableCollection<Article> Articles { get; set; } = new();
    private readonly ObservableCollection<Article> m_OnlineArticles = new();
    private readonly ObservableCollection<Article> m_OfflineArticles;
    private ObservableCollection<Article> m_ArticlesSource;
    
    private MainPageParserFactory MainPageParserFactory { get; } = new();
    
    private Article? _selectedArticle;
    public Article? SelectedArticle
    {
        get => _selectedArticle;

        set
        {
            if (_selectedArticle != value)
            {
                _selectedArticle = value;

                OnPropertyChanged();

                if (_selectedArticle != null)
                {
                    ReadArticleCommand(_selectedArticle);
                }
            }
        }
    }

    private Color _onlineBorderColor = Colors.LightGreen;
    public Color OnlineBorderColor
    {
        get => _onlineBorderColor;
        set
        {
            _onlineBorderColor = value;
            OnPropertyChanged();
        }
    }

    private Color _offlineBorderColor = Colors.LightGray;
    public Color OfflineBorderColor
    {
        get => _offlineBorderColor;
        set
        {
            _offlineBorderColor = value;
            OnPropertyChanged();
        }
    }

    private readonly int m_DisplayableArticles = 15;

    private bool m_AnyArticlesToLoad;
    public bool AnyArticlesToLoad
    {
        get => m_AnyArticlesToLoad;

        set
        {
            m_AnyArticlesToLoad = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    private bool isRefreshing;
    
    public TrendingViewModel(IConnectivity i_Connectivity)
    {
        initializeCommands();
        m_Connectivity = i_Connectivity;
        m_OfflineArticles = OfflineService.LocallyStoredArticles;
        m_ArticlesSource = m_OnlineArticles;
    }

    private void initializeCommands()
    {
        OnlineClickedCommand = new AsyncCommand(OnOnlineTapped);
        OfflineClickedCommand = new Command(OnOfflineTapped);
        ArticleSelectedCommand = new Command<Article>(ReadArticleCommand);
        LoadMoreArticlesCommand = new Command(loadMoreArticlesCommand);
    }

    [RelayCommand]
    public async Task GetArticlesAsync()
    {
        if (IsBusy) return;
        
        try
        {
            if (!IsRefreshing)
            {
                IsBusy = true;
            }
            
            List<Article> articles = await loadOnlineArticles();

            removeDuplicateArticles(ref articles);

            if (areArticlesDifferent(articles))
            {
                updateArticlesSource(articles);
                updateMainCollection();
            }
        }

        finally
        {
            IsRefreshing = false;
            IsBusy = false;
            AnyArticlesToLoad = m_ArticlesSource.Count > Articles.Count;
        }
    }

    private async void ReadArticleCommand(Article i_Article)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            
            if (isOnlineSelected())
            {
                Article? cachedArticle = CacheService.FindCachedArticle(i_Article);
                Article? storedArticle = OfflineService.FindStoredArticle(i_Article);
                SharedData.Cached = cachedArticle is not null;
                SharedData.Stored = storedArticle is not null;

                if (SharedData.Cached) 
                    SharedData.ParsedArticle = cachedArticle;
                
                else if (SharedData.Stored) 
                    SharedData.ParsedArticle = storedArticle;

                else
                {
                    SharedData.SharedArticle = i_Article;
                    SharedData.HTML = await HTMLSupplier.GetHTMLAsync(i_Article.URL);
                }
            }

            else
            {
                Article? storedArticle = OfflineService.FindStoredArticle(i_Article);
                if (storedArticle is null) return;
                SharedData.ParsedArticle = storedArticle;
                SharedData.Stored = true;
            }


            await Shell.Current.GoToAsync(nameof(ReadingPage), true);
        }

        finally
        {
            SelectedArticle = null;
            IsBusy = false;
        }
    }

    private async void readArticleFromOnlineSection(Article i_Article)
    {
        Article? cachedArticle = CacheService.FindCachedArticle(i_Article);
        SharedData.Cached = cachedArticle is not null;
        
        if (cachedArticle is not null)
        {
            Debug.WriteLine("Article is cached!");
            SharedData.ParsedArticle = cachedArticle;
        }
        
        else
        {
            Debug.WriteLine("Article is not cached!");
            SharedData.SharedArticle = i_Article;
            SharedData.HTML = await HTMLSupplier.GetHTMLAsync(i_Article.URL);
            Debug.WriteLine($"Got HTML: {SharedData.HTML.Length}");
        }

        SharedData.Stored = false;
    }

    private void readArticleFromOfflineSection(Article i_Article)
    {
        Article? storedArticle = OfflineService.FindStoredArticle(i_Article);
        
        if (storedArticle is null)
            return;
        
        SharedData.ParsedArticle = storedArticle;
        SharedData.Stored = true;
    }

    private async void handleConnectivity()
    {
        if (Articles.Count == 0 && m_Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("No connectivity!",
                $"Please check internet and try again.", "OK");
            return;
        }

        else if (m_Connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            await Shell.Current.DisplayAlert("No connectivity!",
                $"Please check internet and try again.", "OK");
            return;
        }
    }

    private async Task OnOnlineTapped()
    {
        try
        {
            if (!IsBusy && !isOnlineSelected())
            {
                IsBusy = true;
                OnlineBorderColor = Colors.LightGreen;
                OfflineBorderColor = Colors.LightGray;
                m_ArticlesSource = m_OnlineArticles;

                if (m_OnlineArticles.Count == 0)
                {
                    IsBusy = false;
                    await GetArticlesAsync();
                }

                else
                {
                    updateMainCollection();
                }
            }
        }

        finally
        {
            IsBusy = false;
        }

    }

    private void OnOfflineTapped()
    {
        try
        {
            if (!IsBusy && isOnlineSelected())
            {
                IsBusy = true;
                OfflineBorderColor = Colors.LightGreen;
                OnlineBorderColor = Colors.LightGray;
                m_ArticlesSource = m_OfflineArticles;
                updateMainCollection();
            }
        }
        
        finally
        {
            IsBusy = false;
        }
    }

    private bool isOnlineSelected()
    {
        return OnlineBorderColor.Equals(Colors.LightGreen);
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

        if (Articles.Count == 0 || Articles.Count != i_Articles.Count)
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

        for (int i = 0; i < Articles.Count && !foundArticle; i++)
        {
            if (i_Article.OuterTitle.Equals(m_ArticlesSource[i].OuterTitle))
            {
                foundArticle = true;
            }
        }

        return foundArticle;
    }

    private void loadMoreArticlesCommand()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            int loaded = Articles.Count;

            for (int i = loaded; i < m_ArticlesSource.Count && i < loaded + m_DisplayableArticles; i++)
            {
                Articles.Add(m_ArticlesSource[i]);
            }

            AnyArticlesToLoad = m_ArticlesSource.Count > Articles.Count;
        }

        finally
        {
            IsBusy = false;
        }
    }

    private async Task<List<Article>> loadOnlineArticles()
    {
        Debug.WriteLine("At loadOnlineArticles!");
        handleConnectivity();
        Debug.WriteLine("At loadOnlineArticles!");
        List<Article> articles = new List<Article>();
        List<string> selectedURLs = ConfigService.LoadSupportedWebsites();
        Debug.WriteLine("At loadOnlineArticles!");

        foreach (string webURL in selectedURLs)
        {
            string htmlCode = await HTMLSupplier.GetHTMLAsync(webURL);
            IMainPageParser mainPageParser = MainPageParserFactory.GenerateMainPageParser
                (SharedData.Pairs.FirstOrDefault(x => x.Value == webURL).Key);
            List<Article> websiteArticles = mainPageParser.ParseHTML(htmlCode);

            foreach (Article article in websiteArticles)
            {
                articles.Add(article);
            }
        }

        removeDuplicateArticles(ref articles);

        return articles;
    }

    private void updateMainCollection()
    {
        Articles.Clear();

        for (int i = 0; i < m_DisplayableArticles && i < m_ArticlesSource.Count; i++)
        {
            Articles.Add(m_ArticlesSource[i]);
        }

        AnyArticlesToLoad = m_ArticlesSource.Count > Articles.Count;
        OnPropertyChanged(nameof(Articles));
    }

    private void updateArticlesSource(List<Article> i_Articles)
    {
        m_ArticlesSource.Clear();

        foreach (Article article in i_Articles)
        {
            m_ArticlesSource.Add(article);
        }
    }
}