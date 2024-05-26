namespace OfflineReader.Service;

public class ArticlesService
{
    //public ObservableCollection<Article> Articles { get; set; } = new();


    public async Task<List<Article>> LoadArticles()
    {
        List<Article> articles = new();
        Root myDeserializedClass = await ReadAsync<Root>("OfflineReader.Tests.articles.json");
        //int articleId = 0;

        foreach (ArticleJSONItem item in myDeserializedClass.articles)
        {
            Article article = new()
            {
                Date = item.date,
                Image = item.image,
                Description = item.description,
                Title = item.title,
                SubTitle = item.subTitle,
                ArticleBody = item.articleBody,
                Website = item.website,
                //ArticleId = articleId++
            };

            articles.Add(article);
        }

        return articles;
    }

    //public static async Task<T> ReadAsync<T>(string filePath)
    //{
    //    using FileStream stream = File.OpenRead(filePath);
    //    return await JsonSerializer.DeserializeAsync<T>(stream);
    //}

    public static async Task<T> ReadAsync<T>(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        await using Stream stream = assembly.GetManifestResourceStream(resourcePath);
        if (stream == null) throw new ArgumentException($"No resource found at path: {resourcePath}");
        return await JsonSerializer.DeserializeAsync<T>(stream);
    }


    public class ArticleJSONItem
    {
        public string website { get; set; }
        public string title { get; set; }
        public string subTitle { get; set; }
        public string articleBody { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string date { get; set; }
    }

    public class Root
    {
        public List<ArticleJSONItem> articles { get; set; }
    }
}