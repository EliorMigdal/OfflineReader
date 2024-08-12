using System.Diagnostics;
using System.Windows.Input;
using OfflineReader.Helpers.Content.Generator;
using OfflineReader.Model;
using OfflineReader.Model.HTMLParser.ArticleParser;
using OfflineReader.Service.Local;

namespace OfflineReader.ViewModel;

public class ReaderViewModel : BaseViewModel
{
    private readonly string k_VerifiedImage = "verified.png";
    private readonly string k_ErrorImage = "error.png";
    private readonly string k_DownloadImage = "download.png";
    private readonly string k_DeleteImage = "delete.png";
    public ICommand DownloadButtonCommand { get; private set; }
    public ICommand DeleteButtonCommand { get; private set; }
    private ArticleParserFactory ParserFactory { get; } = new();
    private ArticleContentGenerator ContentGenerator { get; } = new();
    private CacheService CacheService { get; } = CacheService.Instance;
    private OfflineContentService OfflineContentService { get; } = OfflineContentService.Instance;
    private StackLayout _articleLayout;
    public StackLayout ArticleLayout
    {
        get => _articleLayout;
        set => SetProperty(ref _articleLayout, value);
    }

    private bool m_IsDownloadButtonEnabled;
    public bool IsDownloadButtonEnabled
    {
        get => m_IsDownloadButtonEnabled && !IsBusy;
        set
        {
            if (m_IsDownloadButtonEnabled.Equals(value)) return;
            m_IsDownloadButtonEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool IsDeleteButtonEnabled => !IsDownloadButtonEnabled;

    private string m_DownloadButtonImage;
    public string DownloadButtonImage
    {
        get => m_DownloadButtonImage;
        set
        {
            m_DownloadButtonImage = value;
            OnPropertyChanged();
        }
    }
    
    private string m_DeleteButtonImage;
    public string DeleteButtonImage
    {
        get => m_DeleteButtonImage;
        set
        {
            m_DeleteButtonImage = value;
            OnPropertyChanged();
        }
    }

    public ReaderViewModel()
    {
        DownloadButtonImage = k_DownloadImage;
        DeleteButtonImage = k_DeleteImage;
        IsDownloadButtonEnabled = !SharedData.Stored;
        DownloadButtonCommand = new Command(saveArticle);
        DeleteButtonCommand = new Command(deleteArticle);
        initialize();
    }

    private void initialize()
    {
        Article? article;

        if (SharedData.Cached)
        {
            Debug.WriteLine("Article is cached!");
            article = SharedData.ParsedArticle;
        }

        else
        {
            Debug.WriteLine("Article is not cached!");
            article = getParsedArticle();
            Debug.WriteLine("Article is parsed!");
            Task.Run(() => CacheService.CacheArticle(article));
        }

        Debug.Assert(article != null, nameof(article) + " != null");
        ArticleLayout = ContentGenerator.GenerateAView(article);
    }

    private Article getParsedArticle()
    {
        string html = SharedData.HTML;
        Debug.Assert(SharedData.SharedArticle != null, "SharedData.SharedArticle != null");
        string website = SharedData.SharedArticle.Website;
        Debug.WriteLine($"Got website {website}");
        IArticleParser articleParser = ParserFactory.GenerateParser(website);
        Article article = articleParser.ParseHTML(html);
        SharedData.ParsedArticle = article;

        return article;
    }

    private void saveArticle()
    {
        if (IsBusy)
            return;

        bool successfullyStored = false;
        
        Task.Run(async () =>
        {
            try
            {
                IsBusy = true;
                Debug.Assert(SharedData.ParsedArticle != null, "SharedData.ParsedArticle != null");
                successfullyStored = await OfflineContentService.StoreArticle(SharedData.ParsedArticle);
            }
            
            finally
            {
                IsBusy = false;
                IsDownloadButtonEnabled = false;
                DownloadButtonImage = successfullyStored ? k_VerifiedImage : k_ErrorImage;
            }
        });
    }

    private void deleteArticle()
    {
        if (IsBusy)
            return;

        bool successfullyRemoved = false;

        Task.Run(() =>
        {
            try
            {
                IsBusy = true;
                Debug.Assert(SharedData.ParsedArticle != null, "SharedData.ParsedArticle != null");
                successfullyRemoved = OfflineContentService.RemoveArticle(SharedData.ParsedArticle);
            }

            finally
            {
                IsBusy = false;
                IsDownloadButtonEnabled = false;
                DeleteButtonImage = successfullyRemoved ? k_VerifiedImage : k_ErrorImage;
            }
        });
    }
}