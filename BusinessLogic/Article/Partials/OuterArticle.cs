using System.Security.Cryptography;
using System.Text;
using BusinessLogic.Article.Content;

namespace BusinessLogic.Article.Partials;

[Serializable]
public class OuterArticle
{
    public string Title { get; set; } = string.Empty;
    public ImageContent MainImage { get; set; } = new();
    public string Website { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string URL { get; set; } = string.Empty;
    public string ID { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    
    public void GenerateArticleID()
    {
        int timeNumber = generateTimeNumber(LastUpdated);
        string titleID = generateTitleID(Title);
        
        ID = $"{timeNumber}{titleID}";
    }

    private int generateTimeNumber(DateTime i_DateTime)
    {
        return int.Parse(i_DateTime.ToString("HHmm"));
    }

    private string generateTitleID(string i_Title)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(i_Title));
        int hashInt = BitConverter.ToInt32(hashBytes, 0);
        
        return Math.Abs(hashInt).ToString();
    }
}