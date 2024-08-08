using OfflineReader.Model.HTMLParser.ArticleParser;
using OfflineReader.Model;
using OfflineReader.Helpers.Content.Generator;
using OfflineReader.Service.Local;

namespace OfflineReader.ViewModel;

public class TestViewModel : BaseViewModel
{
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

    public bool ButtonIsEnabled { get; set; } = true;

    public TestViewModel()
    {
        _ = initializeAsync();
    }

    private async Task initializeAsync()
    {
        string website = SharedData.SharedArticle.Website;
        Article article;

        if (SharedData.Cached)
        {
            article = SharedData.ParsedArticle;
        }

        else
        {
            string html = SharedData.HTML;
            IArticleParser articleParser = ParserFactory.GenerateParser(website);
            article = articleParser.ParseHTML(html);
            SharedData.ParsedArticle = article;
            _ = Task.Run(() => CacheService.CacheArticle(article));
        }
        
        ArticleLayout = ContentGenerator.generateAView(article);
    }

    public async void SaveArticle(Article i_Article)
    {
        _ = Task.Run(() => OfflineContentService.StoreArticle(i_Article));
    }
}