using System.Diagnostics;
using System.Windows.Input;
using Application.Helpers;
using Application.Helpers.Content.Generator;
using BusinessLogic.HTMLParser.ArticleParser;
using Application.Service.Local;
using AsyncAwaitBestPractices.MVVM;
using BusinessLogic.Article.Partials;

namespace Application.ViewModel;

public class ReaderViewModel : BaseViewModel
{
    private readonly string k_VerifiedImage = "verified.png";
    private readonly string k_ErrorImage = "error.png";
    private readonly string k_DownloadImage = "download.png";
    private readonly string k_DeleteImage = "delete.png";
    public ICommand DownloadButtonCommand { get; private set; }
    public ICommand DeleteButtonCommand { get; private set; }
    private ArticleParserFactory ParserFactory { get; } = ArticleParserFactory.Instance;
    private ArticleContentGenerator ContentGenerator { get; } = ArticleContentGenerator.Instance;
    private InnerArticle? m_ParsedArticle = new();
    private CacheService CacheService { get; } = CacheService.Instance;
    private ConnectivityManager ConnectivityManager { get; } = ConnectivityManager.Instance;
    private OfflineContentService OfflineContentService { get; } = OfflineContentService.Instance;
    private StackLayout _articleLayout = new();
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
            OnPropertyChanged(nameof(IsDeleteButtonEnabled));
        }
    }

    public bool IsDeleteButtonEnabled { get; set; }

    private string m_DownloadButtonImage = string.Empty;
    public string DownloadButtonImage
    {
        get => m_DownloadButtonImage;
        set
        {
            m_DownloadButtonImage = value;
            OnPropertyChanged();
        }
    }
    
    private string m_DeleteButtonImage = string.Empty;
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
        IsDownloadButtonEnabled = !SharedData.IsCurrentArticleStored;
        IsDeleteButtonEnabled = SharedData.IsCurrentArticleStored;
        DownloadButtonCommand = new AsyncCommand(saveArticle);
        DeleteButtonCommand = new Command(deleteArticle);
        ConnectivityManager.ConnectivityChanged += OnConnectivityChanged!;
        initialize();
    }

    private void initialize()
    {
        if (SharedData.IsCurrentArticleCached || SharedData.IsCurrentArticleStored)
        {
            m_ParsedArticle = SharedData.WholeArticle?.InnerArticle;
        }

        else
        {
            m_ParsedArticle = getParsedArticle();
            Task.Run(() => CacheService.CacheArticle(m_ParsedArticle));
        }

        Debug.Assert(m_ParsedArticle != null, nameof(m_ParsedArticle) + " != null");
        ArticleLayout = ContentGenerator.GenerateAView(m_ParsedArticle);
    }

    private InnerArticle? getParsedArticle()
    {
        string html = SharedData.HTML;
        Debug.Assert(SharedData.OuterArticle != null, "SharedData.SharedArticle != null");
        string website = SharedData.OuterArticle.Website;
        Debug.WriteLine($"Got website {website}");
        IArticleParser? articleParser = ParserFactory.GenerateParser(website);
        InnerArticle? article = articleParser?.ParseArticleHTML(html);
        SharedData.InnerArticle = article;

        return article;
    }

    private async Task saveArticle()
    {
        if (IsBusy)
            return;
        
        if (!ConnectivityManager.IsDeviceConnected())
        {
            IsDownloadButtonEnabled = false;
            IsDeleteButtonEnabled = false;
            await ConnectivityManager.AlertConnectivityIssue();
            return;
        }

        bool successfullyStored = false;
        
        await Task.Run(async () =>
        {
            try
            {
                Debug.Assert(m_ParsedArticle is not null, "m_ParsedArticle != null");
                successfullyStored = await OfflineContentService.StoreArticle(m_ParsedArticle);
            }
            
            finally
            {
                IsDownloadButtonEnabled = !successfullyStored;
                IsDeleteButtonEnabled = successfullyStored;
                DownloadButtonImage = successfullyStored ? k_VerifiedImage : k_ErrorImage;
                DeleteButtonImage = k_DeleteImage;
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
                Debug.Assert(SharedData.WholeArticle != null, "SharedData.WholeArticle != null");
                successfullyRemoved = OfflineContentService.RemoveArticle(SharedData.WholeArticle);
            }

            finally
            {
                IsDownloadButtonEnabled = successfullyRemoved;
                IsDeleteButtonEnabled = !successfullyRemoved;
                DeleteButtonImage = successfullyRemoved ? k_VerifiedImage : k_ErrorImage;
                DownloadButtonImage = k_DownloadImage;
            }
        });
    }
    
    private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        IsDownloadButtonEnabled = e.NetworkAccess == NetworkAccess.Internet;
    }
}