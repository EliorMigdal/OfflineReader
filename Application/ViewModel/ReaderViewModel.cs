using System.Diagnostics;
using System.Windows.Input;
using Application.Helpers;
using Application.Helpers.Content.Generator;
using Application.Service.Local;
using AsyncAwaitBestPractices.MVVM;
using BusinessLogic.Article;

namespace Application.ViewModel;

public class ReaderViewModel : BaseViewModel, IQueryAttributable
{
    private AppTheme m_CurrentTheme;
    private readonly string k_VerifiedImage = "verified.png";
    private readonly string k_ErrorImage = "error.png";
    private string m_DownloadImage = string.Empty;
    private string m_DeleteImage = string.Empty;
    
    public ICommand DownloadButtonCommand { get; private set; }
    public ICommand DeleteButtonCommand { get; private set; }
    
    private Article? Article { get; set; }
    private bool IsArticleStored { get; set; }
    private ArticleContentGenerator ContentGenerator { get; } = ArticleContentGenerator.Instance;
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
        initializeButtonImages();
        DownloadButtonCommand = new AsyncCommand(saveArticle);
        DeleteButtonCommand = new Command(deleteArticle);
        ConnectivityManager.ConnectivityChanged += OnConnectivityChanged!;
        
        initializeLayout();
        initializeAppThemeListener();
    }

    private void initializeAppThemeListener()
    {
        Microsoft.Maui.Controls.Application.Current!.RequestedThemeChanged += (_, _) =>
        {
            initializeButtonImages();
        };
    }

    private void initializeButtonImages()
    {
        m_CurrentTheme = Microsoft.Maui.Controls.Application.Current!.RequestedTheme;
        m_DownloadImage = m_CurrentTheme == AppTheme.Light ? "downloadlight.png" : "downloaddark.png";
        m_DeleteImage = m_CurrentTheme == AppTheme.Light ? "deletelight.png" : "deletedark.png";
        
        DownloadButtonImage = m_DownloadImage;
        DeleteButtonImage = m_DeleteImage;
    }

    private void initializeLayout()
    {
        if (Article is not null)
        {
            ArticleLayout = ContentGenerator.GenerateAView(Article.InnerArticle);
        }
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
        
        await Task.Run(() =>
        {
            try
            {
                if (Article is not null)
                {
                    successfullyStored = OfflineContentService.StoreArticle(Article);
                }
            }
            
            finally
            {
                IsDownloadButtonEnabled = !successfullyStored;
                IsDeleteButtonEnabled = successfullyStored;
                DownloadButtonImage = successfullyStored ? k_VerifiedImage : k_ErrorImage;
                DeleteButtonImage = m_DeleteImage;
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
                if (Article is not null)
                {
                    successfullyRemoved = OfflineContentService.RemoveArticle(Article);
                }
            }

            finally
            {
                IsDownloadButtonEnabled = successfullyRemoved;
                IsDeleteButtonEnabled = !successfullyRemoved;
                DeleteButtonImage = successfullyRemoved ? k_VerifiedImage : k_ErrorImage;
                DownloadButtonImage = m_DownloadImage;
            }
        });
    }
    
    private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        IsDownloadButtonEnabled = e.NetworkAccess == NetworkAccess.Internet;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Article = query["Article"] as Article;
        IsArticleStored = (bool) query["IsStored"];
        initializeButtons();
        initializeLayout();
    }

    private void initializeButtons()
    {
        IsDownloadButtonEnabled = !IsArticleStored;
        IsDeleteButtonEnabled = IsArticleStored;
    }
}