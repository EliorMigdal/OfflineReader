using Microsoft.Maui.Controls;
using System.Windows.Input;

namespace OfflineReader.Model;

public class Article
{
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string ArticleBody { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
    public string Image { get; set; }
    public string Date { get; set; }
    public string LastUpdated { get; set; }
    public string URL { get; set; }
    public string ID { get; set; }
}