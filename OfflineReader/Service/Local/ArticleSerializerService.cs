using System.Xml.Serialization;
using OfflineReader.Model;
using System.Diagnostics;

namespace OfflineReader.Service.Local;

public class ArticleSerializerService
{
    private readonly XmlSerializer m_XMLSerializer = new XmlSerializer(typeof(Article));
    private static ArticleSerializerService m_Instance;
    public static ArticleSerializerService Instance
    {
        get
        {
            m_Instance ??= new ArticleSerializerService();

            return m_Instance;
        }
    }

    private ArticleSerializerService() { }

    public void SerializeArticle(Article i_Article, string i_Path)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(i_Path + i_Article.ID + ".xml"))
            {
                Debug.WriteLine($"Serializing at: {i_Path + i_Article.ID + ".xml"}");
                m_XMLSerializer.Serialize(writer, i_Article);
            }
        }

        catch (Exception exception)
        {
            Debug.WriteLine($"Exception: {exception}");
        }
    }

    public Article DeserializeArticle(string i_Path)
    {
        using (StreamReader reader = new StreamReader(i_Path))
        {
            return m_XMLSerializer.Deserialize(reader) as Article;
        }
    }
}