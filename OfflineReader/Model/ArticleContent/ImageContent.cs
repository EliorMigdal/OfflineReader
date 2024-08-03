namespace OfflineReader.Model.ArticleContent;

public class ImageContent : BodyContent
{
    public int ImageID { get; private set; }

    public ImageContent(string i_ImageSource, int i_ImageID)
    {
        Content = i_ImageSource;
        ImageID = i_ImageID;
    }
}