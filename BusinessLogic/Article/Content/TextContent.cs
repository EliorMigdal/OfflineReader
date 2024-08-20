namespace BusinessLogic.Article.Content;

[Serializable]
public class TextContent : BodyContent
{
    public TextContent () { }

    public TextContent(string i_Text)
    {
        Content = i_Text;
    }
}