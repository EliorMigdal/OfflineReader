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

namespace OfflineReader.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ObservableCollection<Article> Articles { get; set; } = new();
    private HTMLSupplierService HTMLSupplier { get; set; } = new();
    private ArticleParserFactory ArticleParserFactory { get; set; } = new();
    private MainPageParserFactory MainPageParserFactory { get; set; } = new();
    private readonly IConnectivity m_Connectivity;
    private readonly Timer configFileCheckTimer;
    private DateTime lastConfigFileWriteTime;
    public ICommand ArticleSelectedCommand { get; }
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

    [ObservableProperty]
    bool isRefreshing;

    public MainViewModel(IConnectivity i_Connectivity)
    {
        ArticleSelectedCommand = new Command<Article>(testCommand);
        m_Connectivity = i_Connectivity;

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
            handleConnectivity();

            List<string> selectedURLs = ConfigService.LoadSupportedWebsites();
            List<Article> articles = new();

            Debug.WriteLine($"Got selected websites size: {selectedURLs.Count}");

            foreach (string webURL in selectedURLs)
            {
                Debug.WriteLine($"Handling selected website: {webURL}");
                string htmlCode = await HTMLSupplier.GetHTMLAsync(webURL);
                IMainPageParser mainPageParser = MainPageParserFactory.GenerateMainPageParser("mako");
                List<Article> websiteArticles = mainPageParser.ParseHTML(htmlCode);

                Debug.WriteLine($"Got {websiteArticles.Count} articles from {webURL}");

                foreach (Article article in websiteArticles)
                {
                    articles.Add(article);
                }
            }

            Articles.Clear();

            foreach (Article article in articles)
            {
                Articles.Add(article);
            }

            OnPropertyChanged(nameof(Articles));
        }

        finally
        {
            IsRefreshing = false;
            IsBusy = false;
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

                IsBusy = false;
                await Shell.Current.GoToAsync(nameof(TestView), true);
            }
        }

        finally
        {
            SelectedArticle = null;
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
}