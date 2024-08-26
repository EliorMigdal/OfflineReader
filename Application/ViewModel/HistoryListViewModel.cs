using System.Collections.ObjectModel;
using System.Windows.Input;
using Application.Service.Hybrid;
using Application.View;
using AsyncAwaitBestPractices.MVVM;
using BusinessLogic.Article;
using BusinessLogic.Article.Partials;

namespace Application.ViewModel;

public class HistoryListViewModel : BaseViewModel, IQueryAttributable
{
    public ObservableCollection<OuterArticle> Articles { get; set; } = new();
    public OuterArticle? SelectedArticle { get; set; }
    public ICommand SelectedArticleCommand { get; set; }
    public string Header { get; set; } = string.Empty;
    private string? Website { get; set; }
    private string? Date { get; set; }
    private readonly ArticleProviderService rm_ArticleProvider = ArticleProviderService.Instance;

    public HistoryListViewModel()
    {
        SelectedArticleCommand = new AsyncCommand(onArticleSelectionCommand);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (Articles.Count != 0) return;
        Website = query["Website"] as string;
        Date = query["Date"] as string;
        List<OuterArticle>? articles = query["Articles"] as List<OuterArticle>;
        articles?.ForEach(article => Articles.Add(article));
        Header = $"{Website} of date {Date}";
        
        OnPropertyChanged(nameof(Header));
        OnPropertyChanged(nameof(Articles));
    }

    private async Task onArticleSelectionCommand()
    {
        try
        {
            if (SelectedArticle is not null)
            {
                (Article? article, bool isStored) = await rm_ArticleProvider.ProvideArticle(SelectedArticle);

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
        }
        
        finally
        {
            clearArticleSelection();
        }
    }

    private void clearArticleSelection()
    {
        SelectedArticle = null;
        OnPropertyChanged(nameof(SelectedArticle));
    }
}