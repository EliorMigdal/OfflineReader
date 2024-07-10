using System.Windows.Input;
using OfflineReader.Model.HTMLParser;

namespace OfflineReader.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ArticlesService ArticlesService { get; set; } = new();
    public ObservableCollection<Article> Articles { get; set; } = new();
    private HTMLSupplierService HTMLSupplier { get; set; } = new();
    private ArticleParserFactory ArticleParserFactory { get; set; } = new();
    private MainPageParserFactory MainPageParserFactory { get; set; } = new();
    private IConnectivity m_Connectivity;
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

    public MainViewModel(IConnectivity i_Connectivity)
    {
        ArticleSelectedCommand = new Command<Article>(testCommand);
        m_Connectivity = i_Connectivity;
    }

    [ObservableProperty]
    bool isRefreshing;

    [RelayCommand]
    public async Task GetArticlesAsync()
    {
        if (IsBusy)
            return;

        try
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

            string htmlCode = await HTMLSupplier.GetHTMLAsync("https://www.mako.co.il/");
            IMainPageParser mainPageParser = MainPageParserFactory.GenerateMainPageParser("mako");
            List<Article> articles = mainPageParser.ParseHTML(htmlCode);

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
                StackLayout ArticleLayout = articleParser.ParseHTML(htmlCode);

                IsBusy = false;
                await Shell.Current.GoToAsync(nameof(TestView), true);
            }
        }

        finally
        {
            SelectedArticle = null;
        }
    }
}