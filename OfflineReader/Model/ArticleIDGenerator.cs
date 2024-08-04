using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace OfflineReader.Model;

public class ArticleIDGenerator
{
    private static ArticleIDGenerator m_Instance;
    public static ArticleIDGenerator Instance
    {
        get
        {
            m_Instance ??= new ArticleIDGenerator();

            return m_Instance;
        }
    }

    public string GenerateArticleID(Article i_Article)
    {
        int timeNumber = generateTimeNumber(i_Article.LastUpdated);
        //string authorID = generateAuthorID(i_Article.Author.Name);
        Debug.WriteLine($"Got unique ID {timeNumber} with date {i_Article.LastUpdated}");
        return $"{timeNumber}";
    }

    private int generateTimeNumber(DateTime i_DateTime)
    {
        return int.Parse(i_DateTime.ToString("HHmm"));
    }

    private string generateAuthorID(string i_AuthorName)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(i_AuthorName));
            int hashInt = BitConverter.ToInt32(hashBytes, 0);
            return Math.Abs(hashInt).ToString();
        }
    }
}