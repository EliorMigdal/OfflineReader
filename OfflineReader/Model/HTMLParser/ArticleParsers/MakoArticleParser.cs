using HtmlAgilityPack;

namespace OfflineReader.Model.HTMLParser.ArticleParsers;

public class MakoArticleParser : IArticleParser
{
    [Obsolete]
    public StackLayout ParseHTML(string i_HTML)
    {
        HtmlDocument htmlDocument = new();
        StackLayout articleLayout = new StackLayout
        {
            Padding = new Thickness(10),
            Margin = new Thickness(10),
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand
        };

        htmlDocument.LoadHtml(i_HTML);
        extractArticleTitle(articleLayout, htmlDocument);
        extractArticleDescription(articleLayout, htmlDocument);
        extractArticleAuthorInfo(articleLayout, htmlDocument);
        extractArticleDatesInfo(articleLayout, htmlDocument);
        extractHeaderImage(articleLayout, htmlDocument);
        extractArticleBody(articleLayout, htmlDocument);

        return articleLayout;
    }

    private void extractArticleTitle(StackLayout io_ArticleLayout, HtmlDocument i_HTML)
    {
        HtmlNode headlineNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='headline']");

        if (headlineNode != null && headlineNode.Attributes["content"] != null)
        {
            string headline = HtmlEntity.DeEntitize(headlineNode.Attributes["content"].Value);

            io_ArticleLayout.Children.Add(new Label
            {
                Text = headline,
                FontSize = 32,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 0, 0, 10),
                FlowDirection = FlowDirection.RightToLeft,
                HorizontalTextAlignment = TextAlignment.Center
            });
        }
    }

    private void extractArticleDescription(StackLayout io_ArticleLayout, HtmlDocument i_HTML)
    {
        HtmlNode descriptionNode = i_HTML.DocumentNode.SelectSingleNode("//meta[@name='description']");

        if (descriptionNode != null)
        {
            string descriptionContent = descriptionNode.GetAttributeValue("content", string.Empty);

            if (!string.IsNullOrEmpty(descriptionContent))
            {
                io_ArticleLayout.Add(new Label
                {
                    Text = HtmlEntity.DeEntitize(descriptionContent),
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 10),
                    FlowDirection = FlowDirection.RightToLeft,
                    HorizontalTextAlignment = TextAlignment.Center
                });
            }
        }
    }

    private void extractArticleAuthorInfo(StackLayout io_ArticleLayout, HtmlDocument i_HTML)
    {
        HtmlNode authorInfoNode = i_HTML.DocumentNode.SelectSingleNode("//div[@class='writer-data']");

        if (authorInfoNode != null)
        {
            HtmlNode authorImageNode = authorInfoNode.SelectSingleNode(".//img[@src]");
            HtmlNode authorNameNode = authorInfoNode.SelectSingleNode(".//a[contains(@href, 'Editor-')]") ?? authorInfoNode.SelectSingleNode(".//span[@itemprop='author']");

            if (authorImageNode != null && authorNameNode != null)
            {
                string authorImageSrc = authorImageNode.GetAttributeValue("src", string.Empty);
                string authorName = HtmlEntity.DeEntitize(authorNameNode.InnerText.Trim());

                io_ArticleLayout.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(0, 0, 0, 10),
                    FlowDirection = FlowDirection.RightToLeft,
                    Children =
                        {
                            new Image
                            {
                                Source = authorImageSrc,
                                HeightRequest = 50,
                                WidthRequest = 50,
                                Aspect = Aspect.AspectFit,
                                Margin = new Thickness(0, 0, 10, 0)
                            },
                            new Label
                            {
                                Text = authorName,
                                FontSize = 16,
                                VerticalOptions = LayoutOptions.Center
                            }
                        }
                });
            }
        }
    }

    private void extractArticleDatesInfo(StackLayout io_ArticleLayout, HtmlDocument i_HTML)
    {
        HtmlNode datePublishedNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='datePublished']");
        HtmlNode dateModifiedNode = i_HTML.DocumentNode.SelectSingleNode("//span[@itemprop='dateModified']");

        if (datePublishedNode != null)
        {
            DateTime datePublished = DateTime.Parse(datePublishedNode.GetAttributeValue("content", string.Empty));

            io_ArticleLayout.Children.Add(new Label
            {
                Text = $"Published: {datePublished:yyyy-MM-dd HH:mm}",
                FontSize = 14,
                TextColor = Colors.Gray,
                Margin = new Thickness(0, 0, 0, 5),
                FlowDirection = FlowDirection.RightToLeft
            });
        }

        if (dateModifiedNode != null)
        {
            DateTime dateModified = DateTime.Parse(dateModifiedNode.GetAttributeValue("content", string.Empty));

            io_ArticleLayout.Children.Add(new Label
            {
                Text = $"Updated: {dateModified:yyyy-MM-dd HH:mm}",
                FontSize = 14,
                TextColor = Colors.Gray,
                Margin = new Thickness(0, 0, 0, 10),
                FlowDirection = FlowDirection.RightToLeft
            });
        }
    }

    private void extractHeaderImage(StackLayout io_ArticleLayout, HtmlDocument i_HTML)
    {
        HtmlNode initialImageNode = i_HTML.DocumentNode.SelectSingleNode("//section[contains(@class, 'article-header i-pic')]/figure");

        if (initialImageNode != null)
        {
            HtmlNode imgNode = initialImageNode.SelectSingleNode(".//img");
            HtmlNode captionNode = initialImageNode.SelectSingleNode(".//figcaption");

            if (imgNode != null)
            {
                string src = imgNode.GetAttributeValue("src", null);
                string description = captionNode != null ? HtmlEntity.DeEntitize(captionNode.InnerText) : string.Empty;

                if (!string.IsNullOrEmpty(src))
                {
                    io_ArticleLayout.Children.Add(new Image
                    {
                        Source = src,
                        HeightRequest = 200,
                        Aspect = Aspect.AspectFit
                    });

                    if (!string.IsNullOrEmpty(description))
                    {
                        io_ArticleLayout.Children.Add(new Label
                        {
                            Text = description,
                            FontSize = 14,
                            TextColor = Colors.Gray,
                            Margin = new Thickness(0, 5, 0, 0),
                            FlowDirection = FlowDirection.RightToLeft
                        });
                    }
                }
            }
        }
    }

    private void extractArticleBody(StackLayout io_ArticleLayout, HtmlDocument i_HTML)
    {
        HtmlNode articleBodyNode = i_HTML.DocumentNode.SelectSingleNode("//section[contains(@class, 'article-body')]");

        if (articleBodyNode != null)
        {
            HtmlNodeCollection contentNodes = articleBodyNode.SelectNodes(".//p[not(.//figure)]|.//figure|.//h4|.//ul[not(@class='tags')]|.//li");

            if (contentNodes != null)
            {
                foreach (HtmlNode node in contentNodes)
                {
                    if (node.Name == "p")
                    {
                        io_ArticleLayout.Children.Add(new Label
                        {
                            Text = HtmlEntity.DeEntitize(node.InnerText),
                            FontSize = 16,
                            Margin = new Thickness(0, 0, 0, 10),
                            FlowDirection = FlowDirection.RightToLeft,
                        });
                    }

                    else if (node.Name == "h4")
                    {
                        io_ArticleLayout.Children.Add(new Label
                        {
                            Text = HtmlEntity.DeEntitize(node.InnerText),
                            FontAttributes = FontAttributes.Bold,
                            FontSize = 20,
                            Margin = new Thickness(0, 0, 0, 10),
                            FlowDirection = FlowDirection.RightToLeft,
                        });
                    }

                    else if (node.Name == "ul")
                    {
                        HtmlNodeCollection listItems = node.SelectNodes(".//li[normalize-space()]");

                        if (listItems != null)
                        {
                            foreach (HtmlNode listItem in listItems)
                            {
                                io_ArticleLayout.Children.Add(new Label
                                {
                                    Text = "• " + HtmlEntity.DeEntitize(listItem.InnerText),
                                    FontSize = 16,
                                    Margin = new Thickness(20, 0, 0, 10),
                                    FlowDirection = FlowDirection.RightToLeft
                                });
                            }
                        }
                    }

                    else if (node.Name == "figure")
                    {
                        HtmlNode imgNode = node.SelectSingleNode(".//img");
                        HtmlNode captionNode = node.SelectSingleNode(".//figcaption");

                        if (imgNode != null)
                        {
                            string src = imgNode.GetAttributeValue("src", null);
                            string description = captionNode != null ? HtmlEntity.DeEntitize(captionNode.InnerText) : string.Empty;

                            if (!string.IsNullOrEmpty(src))
                            {
                                io_ArticleLayout.Children.Add(new Image
                                {
                                    Source = src,
                                    HeightRequest = 200,
                                    Aspect = Aspect.AspectFit
                                });

                                if (!string.IsNullOrEmpty(description))
                                {
                                    io_ArticleLayout.Children.Add(new Label
                                    {
                                        Text = description,
                                        FontSize = 14,
                                        TextColor = Colors.Gray,
                                        Margin = new Thickness(0, 5, 0, 0),
                                        FlowDirection = FlowDirection.RightToLeft
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}