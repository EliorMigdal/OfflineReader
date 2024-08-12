using System.Diagnostics;
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
    public string OuterTitle { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public ImageContent MainImage { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string URL { get; set; } = string.Empty;
    public string ID { get; set; } = string.Empty;

    public string InnerTitle { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Author Author { get; set; } = new();
    public List<BodyContent> ArticleBody { get; set; } = new();
    public DateTime PublishedDate { get; set; }
    public DateTime LastUpdated { get; set; }

    public static Article MergeInnerAndOuterObjects(Article? i_ArticleA, Article i_ArticleB)
    {
        Article? innerArticle;
        Article outerArticle;
        ArticleIDGenerator articleIDGenerator = ArticleIDGenerator.Instance;

        Debug.Assert(i_ArticleA != null, nameof(i_ArticleA) + " != null");
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