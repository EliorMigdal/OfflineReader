using System.Runtime.Serialization.Formatters.Binary;
using OfflineReader.Model;
using System.Diagnostics;

namespace OfflineReader.Service;

public class ArticleSerializerService
{
    BinaryFormatter binaryFormatter = new BinaryFormatter();

    public void SerializeArticle(Article i_Article, string i_Path)
    {
        FileStream fileStream = new FileStream(i_Path, FileMode.Create, FileAccess.Write, FileShare.None);

        try
        {
            using (fileStream)
            {
                #pragma warning disable SYSLIB0011 // Type or member is obsolete
                binaryFormatter.Serialize(fileStream, i_Article);
                #pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }

        catch
        {
            Debug.WriteLine("Serialization failed!");
        }
    }

    public Article DeserializeArticle(string i_Path)
    {
        Article article = null;
        FileStream fileStream = new FileStream(i_Path, FileMode.Open, FileAccess.Read, FileShare.None);

        try
        {
            using (fileStream)
            {
                #pragma warning disable SYSLIB0011
                article = binaryFormatter.Deserialize(fileStream) as Article;
                #pragma warning restore SYSLIB0011
            }
        }

        catch
        {
            Debug.WriteLine("Deserialization failed!");
        }

        return article;
    }
}