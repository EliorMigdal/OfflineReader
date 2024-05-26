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
    public async Task GoToReaderModeAsync(Article i_Article)
    {
        if (i_Article == null)
            return;

        var articleJson = JsonSerializer.Serialize(i_Article);
        var encodedArticleJson = WebUtility.UrlEncode(articleJson);
        await Shell.Current.GoToAsync($"{nameof(ReaderPage)}?article={encodedArticleJson}", true);
    }

}

    //[RelayCommand]
    //async Task GoToReaderModeAsync(Article i_Article)
    //{
    //    if (i_Article == null)
    //        return;

//    await Shell.Current.GoToAsync($"{nameof(ReaderPage)}?articleId={i_Article.ArticleId}", true);

//    //await Shell.Current.GoToAsync(nameof(ReaderPage), true, new Dictionary<string, object>
//    //{
//    //    { "ArticleId", i_Article.ArticleId}
//    //});
//}

//public static Article GetArticleById(int i_ArticleId)
//{
//    Article article = Articles.FirstOrDefault(article => article.ArticleId == i_ArticleId); ;
//    return Articles.FirstOrDefault(article => article.ArticleId == i_ArticleId);
//}

//public static async Task<Article> GetArticleByIdAsync(int i_ArticleId)

//public static async Task<Article> GetArticleByIdAsync(int i_ArticleId)
//{
//    var article = Articles.FirstOrDefault(a => a.ArticleId == i_ArticleId);
//    return Task.FromResult(article);
//    //return await Articles.FirstOrDefault(article => article.ArticleId == i_ArticleId);
//}
//}


//public partial class MainViewModel : BaseViewModel
//{
//    private ArticlesService articleService;
//    public ObservableCollection<Article> Articles { get; set; } = new();

//    public MainViewModel(ArticlesService i_articleService)
//    {
//        articleService = i_articleService;
//        Articles = articleService.Articles;
//    }

//    [RelayCommand]
//    public async Task GetArticlesAsync()
//    {
//        List<Article> articles = await ArticlesService.LoadArticles();

//        foreach (Article article in articles)
//        {
//            Articles.Add(article);
//        }
//    }
//}