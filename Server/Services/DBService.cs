using BusinessLogic.Article.Content;
using BusinessLogic.Article.Partials;
using BusinessLogic.HTMLParser.MainPageParser;
using Npgsql;
using BusinessLogic.SupportedWebsite;

namespace Server.Services;

public class DBService
{
    private static DBService? m_Instance;
    public static DBService Instance
    {
        get
        {
            m_Instance ??= new DBService();
            
            return m_Instance;
        }
    }
    
    private DBService() {}
    
    public List<SupportedWebsite> GetSupportedWebsites()
    {
        List<SupportedWebsite> supportedWebsites = new List<SupportedWebsite>();
        using NpgsqlConnection connection = connectToDatabase();
        string query = "SELECT * FROM SupportedWebsites";
        using var command = new NpgsqlCommand(query, connection);
        
        connection.Open();
        using var reader = command.ExecuteReader();
    
        while (reader.Read())
        {
            var website = new SupportedWebsite
            {
                Name = reader.GetString(reader.GetOrdinal("name")),
                URL = reader.GetString(reader.GetOrdinal("url"))
            };
            
            supportedWebsites.Add(website);
        }

        return supportedWebsites;
    }

    public void AddSupportedWebsite(string i_Name, string i_URL)
    {
        using NpgsqlConnection connection = connectToDatabase();
        string query = "INSERT INTO SupportedWebsites (name, url) VALUES (@name, @url)";
        using var command = new NpgsqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@name", i_Name);
        command.Parameters.AddWithValue("@url", i_URL);
        
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void RemoveSupportedWebsite(string i_Name, string i_URL)
    {
        throw new NotImplementedException();
    }

    public List<OuterArticle> GetArticlesList(string i_Website, string i_Date)
    {
        List<OuterArticle> articles = new List<OuterArticle>();
        using NpgsqlConnection connection = connectToDatabase();
        string query = "SELECT * FROM articles WHERE website = @website and date = @date";
        using var command = new NpgsqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@website", i_Website);
        command.Parameters.AddWithValue("@date", i_Date);
        
        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            OuterArticle website = new OuterArticle
            {
                Website = reader.GetString(reader.GetOrdinal("name")),
                Date = reader.GetString(reader.GetOrdinal("date")),
                Title = reader.GetString(reader.GetOrdinal("title")),
                URL = reader.GetString(reader.GetOrdinal("url")),
                MainImage = new ImageContent(reader.GetString(reader.GetOrdinal("date")), 0)
            };

            articles.Add(website);
        }

        return articles;
    }

    public async Task UpdateArticlesHistory()
    {
        List<SupportedWebsite> supportedWebsites = GetSupportedWebsites();
        MainPageParserFactory factory = MainPageParserFactory.Instance;

        foreach (SupportedWebsite supportedWebsite in supportedWebsites)
        {
            IMainPageParser? parser = factory.GenerateMainPageParser(supportedWebsite.Name);
            if (parser is null) return;
            using HttpClient client = new HttpClient();
            string HTML = await client.GetStringAsync(supportedWebsite.URL);
            List<OuterArticle> parsedList = parser.ParseMainPageHTML(HTML);

            foreach (OuterArticle article in parsedList)
            {
                article.GenerateArticleID();
                insertArticleToTable(article);
            }
        }
    }

    private void insertArticleToTable(OuterArticle i_Article)
    {
        using NpgsqlConnection connection = connectToDatabase();
        string query = "INSERT INTO articles (website, date, title, id, image, url) " +
                       "VALUES (@website, @date, @title, @id, @image, @url)";
        using var command = new NpgsqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@website", i_Article.Website);
        command.Parameters.AddWithValue("@date", i_Article.Date);
        command.Parameters.AddWithValue("@title", i_Article.Title);
        command.Parameters.AddWithValue("@id", i_Article.ID);
        command.Parameters.AddWithValue("@image", i_Article.MainImage.Content);
        command.Parameters.AddWithValue("@url", i_Article.URL);
        
        connection.Open();
        command.ExecuteNonQuery();
    }
    
    private NpgsqlConnection connectToDatabase()
    {
        var configurationBuilder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();

        IConfiguration config = configurationBuilder.Build();
        string? connectionString = config["ConnectionStrings:DefaultConnection"];
        
        return new NpgsqlConnection(connectionString);
    }
}