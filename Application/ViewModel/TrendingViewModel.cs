using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Application.Helpers;
using Application.Service.Hybrid;
using Application.Service.Local;
using Application.Service.Remote;
using Application.View;
using AsyncAwaitBestPractices.MVVM;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.ViewModel;

public partial class TrendingViewModel : BaseViewModel
{
    public ICommand OnlineClickedCommand { get; private set; } = null!;
    public ICommand OfflineClickedCommand { get; private set; } = null!;
    public ICommand ArticleSelectedCommand { get; private set; } = null!;
    public ICommand LoadMoreArticlesCommand { get; private set; } = null!;
    public ICommand RefreshCommand { get; private set; } = null!;
    
    public OuterArticle? SelectedArticle { get; set; }

    private OnlineContentService OnlineService { get; } = OnlineContentService.Instance;
    private OfflineContentService OfflineService { get; } = OfflineContentService.Instance;
    private ConnectivityManager ConnectivityManager { get; } = ConnectivityManager.Instance;
    private readonly ArticleProviderService rm_ArticleProvider = ArticleProviderService.Instance;

    public ObservableCollection<OuterArticle> Articles { get; set; } = new();
    private readonly ObservableCollection<OuterArticle> m_OnlineArticles;
    private readonly ObservableCollection<OuterArticle> m_OfflineArticles;
    private ObservableCollection<OuterArticle> m_ArticlesSource = new();

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
    
    public TrendingViewModel()
    {
        initializeCommands();
        
        m_OnlineArticles = OnlineService.OnlineArticlesList;
        m_OfflineArticles = OfflineService.LocallyStoredOuterArticles;

        initializeOnStartup();
    }

    private void initializeCommands()
    {
        OnlineClickedCommand = new AsyncCommand(onOnlineTappedCommand);
        OfflineClickedCommand = new Command(onOfflineTappedCommand);
        ArticleSelectedCommand = new AsyncCommand(onReadArticleCommand);
        LoadMoreArticlesCommand = new Command(onLoadMoreArticlesCommand);
        RefreshCommand = new AsyncCommand(onRefreshCommand);
    }

    private async void initializeOnStartup()
    {
        if (ConnectivityManager.IsDeviceConnected())
        {
            try
            {
                await OnlineService.UpdateArticlesList();
                m_ArticlesSource = m_OnlineArticles;
            }
            
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error",
                    "Fetching articles has failed.", "OK");
            }
        }

        else
        {
            OfflineService.InitializeOfflineArticles();
            m_ArticlesSource = m_OfflineArticles;
        }
        
        updateMainCollection();
        handleButtonsColors();
    }

    private async Task onReadArticleCommand()
    {
        if (IsBusy || SelectedArticle is null)
            return;

        try
        {
            IsBusy = true;
            Article? article;
            bool isStored;
            
            if (isOnlineSelected())
            {
                (article, isStored) = await rm_ArticleProvider.ProvideArticle(SelectedArticle);
            }

            else
            {
                article = OfflineService.FindStoredArticle(SelectedArticle);
                isStored = true;
            }

            if (article is not null)
            {
                await Shell.Current.GoToAsync(nameof(ReadingPage), true, new Dictionary<string, object>
                {
                    {"Article", article}, {"IsStored", isStored}
                });
            }
            
            else
            {
                await Shell.Current.DisplayAlert("Error",
                    "Fetching article has failed.", "OK");
            }
        }

        finally
        {
            IsBusy = false;
            clearSelectedArticle();
        }
    }

    private async Task onRefreshCommand()
    {
        if (IsBusy) return;

        try
        {
            IsRefreshing = true;

            if (isOnlineSelected() && ConnectivityManager.IsDeviceConnected())
            {
                try
                {
                    await OnlineService.UpdateArticlesList();
                }
                
                catch (Exception)
                {
                    await Shell.Current.DisplayAlert("Error",
                        "Fetching articles has failed.", "OK");
                }
            }
            
            else if (isOnlineSelected() && !ConnectivityManager.IsDeviceConnected())
            {
                await ConnectivityManager.AlertConnectivityIssue();
            }

            updateMainCollection();
        }
        
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task onOnlineTappedCommand()
    {
        try
        {
            if (!IsBusy && !isOnlineSelected())
            {
                IsBusy = true;
                
                if (!ConnectivityManager.IsDeviceConnected())
                {
                    await ConnectivityManager.AlertConnectivityIssue();
                }

                else
                {
                    m_ArticlesSource = m_OnlineArticles;
                    handleButtonsColors();

                    if (m_OnlineArticles.Count == 0)
                    {
                        try
                        {
                            await OnlineService.UpdateArticlesList();
                        }
                        
                        catch (Exception)
                        {
                            await Shell.Current.DisplayAlert("Error",
                                "Fetching articles has failed.", "OK");
                        }
                    }
                    
                    updateMainCollection();
                }
            }
        }

        finally
        {
            IsBusy = false;
        }
    }

    private void onOfflineTappedCommand()
    {
        try
        {
            if (IsBusy || !isOnlineSelected()) return;
            
            IsBusy = true;
            m_ArticlesSource = m_OfflineArticles;
            handleButtonsColors();
            
            if (m_OfflineArticles.Count == 0) OfflineService.InitializeOfflineArticles();
            updateMainCollection();
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
    
    private void onLoadMoreArticlesCommand()
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

    private void handleButtonsColors()
    {
        if (m_ArticlesSource == m_OnlineArticles)
        {
            OnlineBorderColor = Colors.LightGreen;
            OfflineBorderColor = Colors.LightGray;
        }

        else
        {
            OfflineBorderColor = Colors.LightGreen;
            OnlineBorderColor = Colors.LightGray;
        }
    }

    private void clearSelectedArticle()
    {
        SelectedArticle = null;
        OnPropertyChanged(nameof(SelectedArticle));
    }
}