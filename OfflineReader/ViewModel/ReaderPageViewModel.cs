using System.Net;

namespace OfflineReader.ViewModel;

[QueryProperty(nameof(ArticleJson), "article")]
public partial class ReaderPageViewModel : BaseViewModel
{
    private string articleJson;
    public string ArticleJson
    {
        get => articleJson;
        set
        {
            articleJson = value;
            LoadArticle(value);
        }
    }

    private Article singleArticle;
    public Article SingleArticle
    {
        get => singleArticle;
        set
        {
            singleArticle = value;
            OnPropertyChanged(nameof(SingleArticle));
        }
    }

    public void LoadArticle(string articleJson)
    {
        SingleArticle = JsonSerializer.Deserialize<Article>(WebUtility.UrlDecode(articleJson));
    }
}