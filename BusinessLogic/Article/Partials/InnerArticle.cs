using BusinessLogic.Article.Content;

namespace BusinessLogic.Article.Partials;

[Serializable]
public class InnerArticle
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Author Author { get; set; } = new();
    public DateTime PublishedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<BodyContent> BodyContents { get; set; } = new();
}