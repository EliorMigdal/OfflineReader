using System.Xml.Serialization;
using BusinessLogic.Article.Content;
using BusinessLogic.Article.Content.TextType;
using BusinessLogic.Article.Partials;

namespace BusinessLogic.Article;

[Serializable]
[XmlInclude(typeof(BodyContent))]
[XmlInclude(typeof(TextContent))]
[XmlInclude(typeof(ImageContent))]
[XmlInclude(typeof(ImageCredit))]
[XmlInclude(typeof(RegularText))]
[XmlInclude(typeof(SubHeader))]
[XmlInclude(typeof(TextListItem))]
public class Article
{
    public InnerArticle InnerArticle { get; set; } = new();
    public OuterArticle OuterArticle { get; set; } = new();
    
    public Article() {}

    public Article(InnerArticle i_InnerArticle, OuterArticle i_OuterArticle)
    {
        InnerArticle = i_InnerArticle;
        OuterArticle = i_OuterArticle;
    }

    public void SaveArticle(string i_Path)
    {
        string filePath = i_Path + OuterArticle.ID + ".xml";
        XmlSerializer serializer = new XmlSerializer(typeof(Article));
        using Stream fileStream = new FileStream(filePath, FileMode.Create);
        
        serializer.Serialize(fileStream, this);
    }

    public static Article? LoadArticle(string i_Path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Article));
        using Stream fileStream = new FileStream(i_Path, FileMode.Open);
        Article? article = serializer.Deserialize(fileStream) as Article;

        return article;
    }
}