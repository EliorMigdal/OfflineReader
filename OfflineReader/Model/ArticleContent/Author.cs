namespace OfflineReader.Model.ArticleContent;

[Serializable]
public class Author
{
    public string Name { get; set; }
    public string Image { get; set; } = string.Empty;

    public Author() { }
}