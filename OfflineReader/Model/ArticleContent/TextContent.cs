﻿namespace OfflineReader.Model.ArticleContent;

public class TextContent : BodyContent
{
    public TextContent () { }

    public TextContent(string i_Text)
    {
        Content = i_Text;
    }
}