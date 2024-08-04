using System.Xml.Serialization;
using OfflineReader.Model.ArticleContent;
using OfflineReader.Model.ArticleContent.TextType;

namespace OfflineReader.Model;

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
    public string OuterTitle { get; set; }
    public string Date { get; set; }
    public string Website { get; set; }
    public ImageContent MainImage { get; set; }
    public string Category { get; set; }
    public string URL { get; set; }
    public string ID { get; set; }

    public string InnerTitle { get; set; }
    public string SubTitle { get; set; }
    public string Description { get; set; }
    public Author Author { get; set; }
    public List<BodyContent> ArticleBody { get; set; } = new();
    public DateTime PublishedDate { get; set; }
    public DateTime LastUpdated { get; set; }

    public static Article MergeInnerAndOuterObjects(Article i_ArticleA, Article i_ArticleB)
    {
        Article innerArticle, outerArticle;
        ArticleIDGenerator articleIDGenerator = ArticleIDGenerator.Instance;

        if (i_ArticleA.OuterTitle.Equals(string.Empty))
        {
            outerArticle = i_ArticleB;
            innerArticle = i_ArticleA;
        }

        else
        {
            outerArticle = i_ArticleA;
            innerArticle = i_ArticleB;
        }

        return new Article
        {
            OuterTitle = outerArticle.OuterTitle,
            Date = outerArticle.Date,
            Website = outerArticle.Website,
            MainImage = outerArticle.MainImage,
            InnerTitle = innerArticle.InnerTitle,
            SubTitle = innerArticle.SubTitle,
            Description = innerArticle.Description,
            Author = innerArticle.Author,
            ArticleBody = innerArticle.ArticleBody,
            PublishedDate = innerArticle.PublishedDate,
            LastUpdated = innerArticle.LastUpdated,
            Category = outerArticle.Category,
            URL = outerArticle.URL,
            ID = articleIDGenerator.GenerateArticleID(outerArticle)
        };
    }
}