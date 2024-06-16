using System.Text.Json;

namespace OfflineReader.Service;

public class ArticlesService
{
    public async Task<List<Article>> LoadArticles()
    {
        List<Article> articles = new();
        Root myDeserializedClass = await ReadAsync<Root>("Tests/articles.json");

        foreach (ArticleJSONItem item in myDeserializedClass.articles)
        {
            Article article = new()
            {
                Date = item.date,
                Image = item.image,
                Description = item.description,
                Title = item.title,
                Website = item.website,
                URL = item.url,
                ID = item.id
            };

            articles.Add(article);
        }

        return articles;
    }

    public static async Task<T> ReadAsync<T>(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<T>(stream);
    }

    public class ArticleJSONItem
    {
        public string website { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string date { get; set; }
        public string url { get; set; }
        public string id { get; set; }
    }

    public class Root
    {
        public List<ArticleJSONItem> articles { get; set; }
    }
}