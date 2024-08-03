using OfflineReader.Model.HTMLParser.ArticleParser;
using OfflineReader.Model;
using OfflineReader.Helpers.Content.Generator;
using OfflineReader.Service;
using System.Diagnostics;

namespace OfflineReader.ViewModel;

public class TestViewModel : BaseViewModel
{
    private ArticleParserFactory ParserFactory { get; set; } = new ArticleParserFactory();
    private ArticleContentGenerator ContentGenerator { get; set; } = new ArticleContentGenerator();
    private CacheService CacheService { get; set; } = new CacheService();
    private StackLayout _articleLayout;
    public StackLayout ArticleLayout
    {
        get => _articleLayout;
        set => SetProperty(ref _articleLayout, value);
    }

    public TestViewModel()
    {
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        string website = SharedData.SharedArticle.Website;
        Article article;

        if (SharedData.Cached)
        {
            Debug.WriteLine($"Cached!");
            article = SharedData.SharedArticle;
        }

        else
        {
            Debug.WriteLine($"Not Cached!");
            string html = SharedData.HTML;
            IArticleParser articleParser = ParserFactory.GenerateParser(website);
            article = articleParser.ParseHTML(html);
            Debug.WriteLine($"About to cache the article!");
            _ = Task.Run(() => CacheService.CacheArticle(article));
        }
        
        ArticleLayout = ContentGenerator.generateAView(article);
    }
}