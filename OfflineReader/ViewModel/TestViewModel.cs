using OfflineReader.Model.HTMLParser;
namespace OfflineReader.ViewModel;

public partial class TestViewModel : BaseViewModel
{
    private ArticleParserFactory ParserFactory { get; set; } = new();
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
        ArticleLayout = articleParser.ParseHTML(html);
    }
}