using BusinessLogic.Article.Content;
using BusinessLogic.Article.Partials;
using BusinessLogic.HTMLParser.MainPageParser;
using Npgsql;
using BusinessLogic.SupportedWebsite;

namespace Server.Services;

public sealed class DBService
{
    private static DBService? s_Instance;
    public static DBService Instance
    {
        get
        {
            s_Instance ??= new DBService();
            
            return s_Instance;
        }
    }
    
    private DBService() {}
    
    public List<SupportedWebsite> GetSupportedWebsites()
    {
        List<SupportedWebsite> supportedWebsites = new List<SupportedWebsite>();
        using NpgsqlConnection connection = connectToDatabase();
        const string query = "SELECT * " +
                       "FROM SupportedWebsites";
        using var command = new NpgsqlCommand(query, connection);
        
        connection.Open();
        using var reader = command.ExecuteReader();
    
        while (reader.Read())
        {
            var website = new SupportedWebsite
            {
                Name = reader.GetString(reader.GetOrdinal("name")),
                URL = reader.GetString(reader.GetOrdinal("url")),
                ImageURL = reader.GetString(reader.GetOrdinal("imageurl"))
            };
            
            supportedWebsites.Add(website);
        }

        return supportedWebsites;
    }

    public void AddSupportedWebsite(string i_Name, string i_URL, string i_Logo)
    {
        using NpgsqlConnection connection = connectToDatabase();
        const string query = "INSERT INTO SupportedWebsites (name, url, imageurl) " +
                       "VALUES (@name, @url, @imageurl)";
        using var command = new NpgsqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@name", i_Name);
        command.Parameters.AddWithValue("@url", i_URL);
        command.Parameters.AddWithValue("@imageurl", i_Logo);
        
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void RemoveSupportedWebsite(string i_Name)
    {
        using NpgsqlConnection connection = connectToDatabase();
        const string query = @" DELETE FROM SupportedWebsites 
                            WHERE name = @name";
    
        using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@name", i_Name);
    
        connection.Open(); 
        command.ExecuteNonQuery();
    }

    public List<OuterArticle> GetArticlesList(string i_Website, string i_Date)
    {
        List<OuterArticle> articles = new List<OuterArticle>();
        using NpgsqlConnection connection = connectToDatabase();
    
        if (!DateTime.TryParseExact(i_Date, "dd-MM-yyyy", null, 
                System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
        {
            throw new ArgumentException("Invalid date format");
        }

        const string query = "SELECT * " +
                       "FROM articles " +
                       "WHERE website = @website AND date::date = @date";
        using var command = new NpgsqlCommand(query, connection);
    
        command.Parameters.AddWithValue("@website", i_Website);
        command.Parameters.AddWithValue("@date", parsedDate);
    
        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            DateTime articleDate = reader.GetDateTime(reader.GetOrdinal("date"));
            TimeSpan articleTime = reader.GetTimeSpan(reader.GetOrdinal("time"));

            var dateTime = articleDate.Add(articleTime);
            
            OuterArticle article = new OuterArticle
            {
                Website = reader.GetString(reader.GetOrdinal("website")),
                Date = dateTime,
                Title = reader.GetString(reader.GetOrdinal("title")),
                URL = reader.GetString(reader.GetOrdinal("url")),
                MainImage = new ImageContent(reader.GetString(reader.GetOrdinal("image")), 0),
                ID = reader.GetString(reader.GetOrdinal("id"))
            };

            articles.Add(article);
        }
    
        return articles;
    }

    public async Task UpdateArticlesHistory()
    {
        await insertNewArticles();
        deleteOldArticles();
    }

    private async Task insertNewArticles()
    {
        List<SupportedWebsite> supportedWebsites = GetSupportedWebsites();
        using HttpClient client = new HttpClient();
        
        foreach (SupportedWebsite supportedWebsite in supportedWebsites)
        {
            IMainPageParser? parser = MainPageParserFactory.GenerateMainPageParser(supportedWebsite.Name);
            if (parser is null) continue;
            string HTML = await client.GetStringAsync(supportedWebsite.URL);
            List<OuterArticle> parsedList = parser.ParseMainPageHTML(HTML);

            foreach (OuterArticle article in parsedList)
            {
                InsertArticleToTable(article);
            }
        }
    }

    private void InsertArticleToTable(OuterArticle i_Article)
    {
        using NpgsqlConnection connection = connectToDatabase();
        const string query = @"
                INSERT INTO articles (website, date, title, id, image, url, time) 
                VALUES (@website, @date, @title, @id, @image, @url, @time)
                ON CONFLICT (id) 
                DO UPDATE SET 
                    website = EXCLUDED.website,
                    date = EXCLUDED.date,
                    title = EXCLUDED.title,
                    image = EXCLUDED.image,
                    url = EXCLUDED.url,
                    time = EXCLUDED.time;";
            
        using var command = new NpgsqlCommand(query, connection);
            
        command.Parameters.AddWithValue("@website", i_Article.Website);
        command.Parameters.AddWithValue("@date", i_Article.Date);
        command.Parameters.AddWithValue("@title", i_Article.Title);
        command.Parameters.AddWithValue("@id", i_Article.ID);
        command.Parameters.AddWithValue("@image", i_Article.MainImage.Content);
        command.Parameters.AddWithValue("@url", i_Article.URL);
        command.Parameters.AddWithValue("@time", i_Article.Date);
        
        connection.Open();
        command.ExecuteNonQuery();
    }
    
    private void deleteOldArticles()
    {
        DateTime oneWeekAgo = DateTime.Now.AddDays(-8);

        using NpgsqlConnection connection = connectToDatabase();
        const string query = @"
        DELETE FROM articles 
        WHERE date < @oneWeekAgo";
    
        using var command = new NpgsqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@oneWeekAgo", oneWeekAgo);
        connection.Open(); 
        command.ExecuteNonQuery();
    }
    
    public List<string> GetAvailableDates(string i_Website)
    {
        List<string> dates = new List<string>();
        DateTime yesterday = DateTime.Now.AddDays(-1);
        using NpgsqlConnection connection = connectToDatabase();

        const string query = "SELECT to_char(date, 'DD-MM-YYYY') as formatted_date " +
                        "FROM articles " +
                       "WHERE (website = @website and date <= @yesterday)" +
                       "GROUP BY formatted_date " +
                       "ORDER BY formatted_date";
        using var command = new NpgsqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@website", i_Website);
        command.Parameters.AddWithValue("@yesterday", yesterday);
        connection.Open();
        
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            dates.Add(reader.GetString(reader.GetOrdinal("formatted_date")));
        }
    
        return dates;
    }
    
    private NpgsqlConnection connectToDatabase()
    {
        string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        
        return new NpgsqlConnection(connectionString);
    }

    public void InitializeDatabase()
    {
        using NpgsqlConnection connection = connectToDatabase();
        
        createWebsitesTable(connection);
        createArticlesTable(connection);
    }

    private void createWebsitesTable(NpgsqlConnection i_Connection)
    {
        const string query = @"CREATE TABLE IF NOT EXISTS SupportedWebsites (
                            Name VARCHAR(15) PRIMARY KEY,
                            URL VARCHAR(50),
                            ImageURL VARCHAR);";
        using var command = new NpgsqlCommand(query, i_Connection);
        
        i_Connection.Open();
        command.ExecuteNonQuery();
    }

    private void createArticlesTable(NpgsqlConnection i_Connection)
    {
        const string query = @"CREATE TABLE IF NOT EXISTS Articles (
                            ID VARCHAR PRIMARY KEY,
                            URL VARCHAR,
                            Image VARCHAR,
                            Date DATE,
                            Time TIME,
                            Website VARCHAR(15),
                            Title VARCHAR);";
        using var command = new NpgsqlCommand(query, i_Connection);
        
        command.ExecuteNonQuery();
    }
}