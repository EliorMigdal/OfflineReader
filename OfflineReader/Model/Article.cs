using OfflineReader.Model.ArticleContent;

namespace OfflineReader.Model;

public class Article
{
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public Author Author { get; set; } = new Author();
    public List<BodyContent> ArticleBody { get; set; } = new();
    public string Date { get; set; }
    public DateTime PublishedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    public string Website { get; set; }
    public string Category { get; set; }
    public string Image { get; set; }
    public string URL { get; set; }
    public string ID { get; set; }
}