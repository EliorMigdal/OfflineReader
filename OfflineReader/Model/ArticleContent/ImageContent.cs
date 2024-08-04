namespace OfflineReader.Model.ArticleContent;

[Serializable]
public class ImageContent : BodyContent
{
    public int ImageID { get; set; }

    public ImageContent() { }

    public ImageContent(string i_ImageSource, int i_ImageID)
    {
        Content = i_ImageSource;
        ImageID = i_ImageID;
    }
}