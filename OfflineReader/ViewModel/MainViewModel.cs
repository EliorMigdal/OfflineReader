using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OfflineReader.View;
using OfflineReader.Service.Local;
using OfflineReader.Service.Remote;
using OfflineReader.Model;
using OfflineReader.Model.HTMLParser.ArticleParser;
using OfflineReader.Model.HTMLParser.MainPageParser;
using System.Diagnostics;
using AsyncAwaitBestPractices.MVVM;

namespace OfflineReader.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ICommand OnlineClickedCommand { get; }
    public ICommand OfflineClickedCommand { get; }
    public ICommand ArticleSelectedCommand { get; }
    public ICommand LoadMoreArticlesCommand { get; }

    public ObservableCollection<Article> Articles { get; set; } = new ObservableCollection<Article>();
    private List<Article> m_OnlineArticles = new List<Article>();
    private List<Article> m_OfflineArticles = new List<Article>();
    private List<Article> m_ArticlesSource;

    private HTMLSupplierService HTMLSupplier { get; } = HTMLSupplierService.Instance;
    private CacheService CacheService { get; } = CacheService.Instance;
    private readonly IConnectivity m_Connectivity;
    private readonly Timer configFileCheckTimer;
    private DateTime lastConfigFileWriteTime;

    private ArticleParserFactory ArticleParserFactory { get; } = new ArticleParserFactory();
    private MainPageParserFactory MainPageParserFactory { get; } = new MainPageParserFactory();
    
    private Article _selectedArticle;
    public Article SelectedArticle
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

    private int m_DisplayableArticles = 15;
    public int DisplayableArticles
    {
        get => m_DisplayableArticles;

        set
        {
            m_DisplayableArticles = value;
            OnPropertyChanged();
        }
    }

    private bool m_AnyArticlesToLoad;
    public bool AnyArticlesToLoad
    {
        get => m_ArticlesSource.Count > Articles.Count;

        set
        {
            m_AnyArticlesToLoad = value;
            OnPropertyChanged();
        }
    }

    [ObservableProperty]
    bool isRefreshing;

    public MainViewModel(IConnectivity i_Connectivity)
    {
        OnlineClickedCommand = new AsyncCommand(OnOnlineTapped);
        OfflineClickedCommand = new AsyncCommand(OnOfflineTapped);
        ArticleSelectedCommand = new Command<Article>(ReadArticleCommand);
        LoadMoreArticlesCommand = new Command(loadMoreArticlesCommand);
        m_Connectivity = i_Connectivity;
        m_ArticlesSource = m_OnlineArticles;

        configFileCheckTimer = new Timer(CheckConfigFileChanges, null, 0, 10000);

        if (File.Exists(ConfigService.ConfigFilePath))
        {
            lastConfigFileWriteTime = File.GetLastWriteTime(ConfigService.ConfigFilePath);
        }
    }

    private void CheckConfigFileChanges(object state)
    {
        if (File.Exists(ConfigService.ConfigFilePath))
        {
            DateTime currentWriteTime = File.GetLastWriteTime(ConfigService.ConfigFilePath);

            if (currentWriteTime != lastConfigFileWriteTime)
            {
                lastConfigFileWriteTime = currentWriteTime;
                MainThread.BeginInvokeOnMainThread(async () => await GetArticlesAsync());
            }
        }
    }

    [RelayCommand]
    public async Task GetArticlesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (!IsRefreshing)
            {
                IsBusy = true;
            }
            
            List<Article> articles;

            if (m_ArticlesSource == m_OnlineArticles)
            {
                articles = await loadOnlineArticles();
            }

            else
            {
                articles = await loadOfflineArticles();
            }

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
        if (i_Article is null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            Debug.WriteLine("Searching for cached article!");
            Article cachedArticle = CacheService.FindCachedArticle(i_Article);
            Debug.WriteLine("Done searching for cached article!");

            if (cachedArticle is null)
            {
                Debug.WriteLine("Didn't find cached article!");
                string htmlCode = await HTMLSupplier.GetHTMLAsync(i_Article.URL);
                SharedData.SharedArticle = i_Article;
                SharedData.HTML = htmlCode;
                SharedData.Cached = false;
            }

            else
            {
                Debug.WriteLine("Found cached article!");
                SharedData.SharedArticle = cachedArticle;
                SharedData.HTML = string.Empty;
                SharedData.Cached = true;
            }

            await Shell.Current.GoToAsync(nameof(TestView), true);
        }

        finally
        {
            SelectedArticle = null;
            IsBusy = false;
        }
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

    private async Task OnOfflineTapped()
    {
        Debug.WriteLine("Tapped Offline!");

        if (!IsBusy && isOnlineSelected())
        {
            IsBusy = true;
            OfflineBorderColor = Colors.LightGreen;
            OnlineBorderColor = Colors.LightGray;
            m_ArticlesSource = m_OfflineArticles;
            IsBusy = false;
            await GetArticlesAsync();
        }
    }

    private bool isOnlineSelected()
    {
        return OnlineBorderColor == Colors.LightGreen;
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

    public void loadMoreArticlesCommand()
    {
        if (IsBusy)
            return;

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
        handleConnectivity();
        List<Article> articles = new List<Article>();
        List<string> selectedURLs = ConfigService.LoadSupportedWebsites();

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

    private async Task<List<Article>> loadOfflineArticles()
    {
        List<Article> articles = new List<Article>();

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