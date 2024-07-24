using OfflineReader.Model.HTMLParser.ArticleParser;
using OfflineReader.Model;
using OfflineReader.Helpers.Content.Generator;

namespace OfflineReader.ViewModel;

public class TestViewModel : BaseViewModel
{
    private ArticleParserFactory ParserFactory { get; set; } = new();
    private ArticleContentGenerator ContentGenerator {get; set;} = new();
    private StackLayout _articleLayout;
    public StackLayout ArticleLayout
    {
        get => _articleLayout;
        set => SetProperty(ref _articleLayout, value);
    }

    public TestViewModel()
    {
        string website = SharedData.SharedArticle.Website;
        string html = SharedData.HTML;

        IArticleParser articleParser = ParserFactory.GenerateParser(website);
        Article article = articleParser.ParseHTML(html);
        //cache it
        ArticleLayout = ContentGenerator.generateAView(article);
    }
}