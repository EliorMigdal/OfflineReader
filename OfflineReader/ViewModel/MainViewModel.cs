using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using OfflineReader.View;
using OfflineReader.Service;
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

    private HTMLSupplierService HTMLSupplier { get; set; } = new HTMLSupplierService();
    private ArticleParserFactory ArticleParserFactory { get; set; } = new ArticleParserFactory();
    private MainPageParserFactory MainPageParserFactory { get; set; } = new MainPageParserFactory();
    private readonly IConnectivity m_Connectivity;
    private readonly Timer configFileCheckTimer;
    private DateTime lastConfigFileWriteTime;
    
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
                    testCommand(_selectedArticle);
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
        ArticleSelectedCommand = new Command<Article>(testCommand);
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
            IsBusy = true;
            List<Article> articles;

            if (m_ArticlesSource == m_OnlineArticles)
            {
                Debug.WriteLine("Source is Online!");
                articles = await loadOnlineArticles();
                Debug.WriteLine($"Got {articles.Count} online articles!");
            }

            else
            {
                Debug.WriteLine("Source is Offline!");
                articles = await loadOfflineArticles();
                Debug.WriteLine($"Got {articles.Count} offline articles!");
            }

            removeDuplicateArticles(ref articles);
            Debug.WriteLine("Removed dups!");

            if (areArticlesDifferent(articles))
            {
                Debug.WriteLine("Collections are different");
                m_ArticlesSource.Clear();

                foreach (Article article in articles)
                {
                    m_ArticlesSource.Add(article);
                }

                Articles.Clear();

                for (int i = 0; i < m_DisplayableArticles && i < m_ArticlesSource.Count; i++)
                {
                    Articles.Add(m_ArticlesSource[i]);
                }

                OnPropertyChanged(nameof(Articles));
            }
        }

        finally
        {
            IsRefreshing = false;
            IsBusy = false;
            AnyArticlesToLoad = m_ArticlesSource.Count > Articles.Count;
        }
    }

    [RelayCommand]
    public async Task GoToReaderModeAsync(Article i_Article)
    {
        if (i_Article == null)
            return;

        await Shell.Current.GoToAsync(nameof(ReaderPage), true, new Dictionary<string, object>
        {
            { "Article", i_Article }
        });
    }

    private async void testCommand(Article i_Article)
    {
        if (i_Article is null)
            return;

        try
        {
            if (!IsBusy)
            {
                IsBusy = true;
                string htmlCode = await HTMLSupplier.GetHTMLAsync(i_Article.URL);

                SharedData.SharedArticle = i_Article;
                SharedData.HTML = htmlCode;
                IArticleParser articleParser = ArticleParserFactory.GenerateParser(i_Article.Website.ToLower());

                await Shell.Current.GoToAsync(nameof(TestView), true);
            }
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
        Debug.WriteLine("Tapped Online!");

        if (!IsBusy && !isOnlineSelected())
        {
            IsBusy = true;
            OnlineBorderColor = Colors.LightGreen;
            OfflineBorderColor = Colors.LightGray;
            m_ArticlesSource = m_OnlineArticles;
            IsBusy = false;
            await GetArticlesAsync();
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
            .GroupBy(article => article.Title)
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
            if (i_Article.Title.Equals(m_ArticlesSource[i].Title))
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
            //Debug.WriteLine($"Handling selected website: {webURL}");
            string htmlCode = await HTMLSupplier.GetHTMLAsync(webURL);
            IMainPageParser mainPageParser = MainPageParserFactory.GenerateMainPageParser
                (SharedData.Pairs.FirstOrDefault(x => x.Value == webURL).Key);
            List<Article> websiteArticles = mainPageParser.ParseHTML(htmlCode);
            //Debug.WriteLine($"Got {websiteArticles.Count} articles from {webURL}");

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
}