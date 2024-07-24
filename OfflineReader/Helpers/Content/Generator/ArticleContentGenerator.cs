using OfflineReader.Model;
using OfflineReader.Model.ArticleContent;
using OfflineReader.Model.ArticleContent.TextType;

namespace OfflineReader.Helpers.Content.Generator;

public class ArticleContentGenerator
{
    public StackLayout generateAView(Article i_Article)
    {
        StackLayout articleLayout = new();

        addArticleTitle(articleLayout, i_Article);
        addArticleDescription(articleLayout, i_Article);
        addArticleAuthor(articleLayout, i_Article);
        addArticleDates(articleLayout, i_Article);
        addArticleBody(articleLayout, i_Article);

        return articleLayout;
    }

    private void addArticleTitle(StackLayout io_StackLayout, Article i_Article)
    {
        io_StackLayout.Children.Add(new Label
        {
            Text = i_Article.Title,
            FontSize = 32,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 10),
            FlowDirection = FlowDirection.RightToLeft,
            HorizontalTextAlignment = TextAlignment.Center
        });
    }

    private void addArticleDescription(StackLayout io_StackLayout, Article i_Article)
    {
        io_StackLayout.Add(new Label
        {
            Text = i_Article.Description,
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(0, 0, 0, 10),
            FlowDirection = FlowDirection.RightToLeft,
            HorizontalTextAlignment = TextAlignment.Center
        });
    }

    private void addArticleAuthor(StackLayout io_StackLayout, Article i_Article)
    {
        io_StackLayout.Add(new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 10),
            FlowDirection = FlowDirection.RightToLeft,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children =
            {
                new Image
                {
                    Source = i_Article.Author.Image,
                    HeightRequest = 50,
                    WidthRequest = 50,
                    Aspect = Aspect.AspectFit,
                    Margin = new Thickness(0, 0, 10, 0)
                },
                new Label
                {
                    Text = i_Article.Author.Name,
                    FontSize = 16,
                    VerticalOptions = LayoutOptions.Center
                }
            }
        });
    }

    private void addArticleDates(StackLayout io_StackLayout, Article i_Article)
    {
        io_StackLayout.Children.Add(new Label
        {
            Text = $"Published: {i_Article.PublishedDate:yyyy-MM-dd HH:mm}",
            FontSize = 14,
            TextColor = Colors.Gray,
            Margin = new Thickness(0, 0, 0, 5),
            FlowDirection = FlowDirection.LeftToRight,
            HorizontalOptions = LayoutOptions.Center
        });

        if (!i_Article.PublishedDate.Equals(i_Article.LastUpdated))
        {
            io_StackLayout.Children.Add(new Label
            {
                Text = $"Updated: {i_Article.LastUpdated:yyyy-MM-dd HH:mm}",
                FontSize = 14,
                TextColor = Colors.Gray,
                Margin = new Thickness(0, 0, 0, 5),
                FlowDirection = FlowDirection.LeftToRight,
                HorizontalOptions = LayoutOptions.Center
            });
        }
    }

    private void addArticleBody(StackLayout io_StackLayout, Article i_Article)
    {
        foreach (BodyContent bodyContent in i_Article.ArticleBody)
        {
            if (bodyContent is TextContent)
            {
                if (bodyContent is ImageCredit)
                {
                    io_StackLayout.Children.Add(new Label
                    {
                        Text = bodyContent.Content,
                        FontSize = 10,
                        TextColor = Colors.Gray,
                        Margin = new Thickness(0, 5, 0, 0),
                        FlowDirection = FlowDirection.RightToLeft
                    });
                }

                else if (bodyContent is SubHeader)
                {
                    io_StackLayout.Children.Add(new Label
                    {
                        Text = bodyContent.Content,
                        FontAttributes = FontAttributes.Bold,
                        FontSize = 20,
                        Margin = new Thickness(0, 0, 0, 10),
                        FlowDirection = FlowDirection.RightToLeft,
                    });
                }

                else if (bodyContent is TextListItem)
                {
                    io_StackLayout.Children.Add(new Label
                    {
                        Text = bodyContent.Content,
                        FontSize = 16,
                        Margin = new Thickness(20, 0, 0, 10),
                        FlowDirection = FlowDirection.RightToLeft
                    });
                }

                else if (bodyContent is RegularText)
                {
                    io_StackLayout.Children.Add(new Label
                    {
                        Text = bodyContent.Content,
                        FontSize = 16,
                        Margin = new Thickness(0, 0, 0, 10),
                        FlowDirection = FlowDirection.RightToLeft,
                    });
                }
            }

            else if (bodyContent is ImageContent)
            {
                io_StackLayout.Children.Add(new Image
                {
                    Source = bodyContent.Content,
                    HeightRequest = 200,
                    Aspect = Aspect.AspectFit
                });
            }
        }
    }
}