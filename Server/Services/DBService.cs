using BusinessLogic.Article.Partials;
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

        return articles;
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