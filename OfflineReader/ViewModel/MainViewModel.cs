namespace OfflineReader.ViewModel;

public partial class MainViewModel : BaseViewModel
{
    public ArticlesService ArticlesService { get; set; } = new();
    public ObservableCollection<Article> Articles { get; set; } = new();

    [RelayCommand]
    public async Task GetArticlesAsync()
    {
        List<Article> articles = await ArticlesService.LoadArticles();

        foreach (Article article in articles)
        {
            Articles.Add(article);
        }
    }

    [RelayCommand]
    public async Task GoToReaderPage()
    {
        await Shell.Current.GoToAsync(nameof(ReaderPage));
    }
}