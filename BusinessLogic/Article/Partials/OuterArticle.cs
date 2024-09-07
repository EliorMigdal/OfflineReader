using BusinessLogic.Article.Content;

namespace BusinessLogic.Article.Partials;

[Serializable]
public class OuterArticle
{
    public string Title { get; set; } = string.Empty;
    public ImageContent MainImage { get; set; } = new();
    public string Website { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string URL { get; set; } = string.Empty;
    public string ID { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}