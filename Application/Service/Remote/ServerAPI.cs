using System.Diagnostics;
using System.Text.Json;
using BusinessLogic.Article.Partials;
using BusinessLogic.SupportedWebsite;

namespace Application.Service.Remote;

public sealed class ServerAPI
{
    private readonly string r_BaseURL = "http://20.217.168.73/offlineReader";
    private static readonly object rm_CreationLock = new();
    private readonly JsonSerializerOptions rm_CaseInsensitiveOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private static ServerAPI? m_Instance;
    public static ServerAPI Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ServerAPI();

                return m_Instance;
            }
        }
    }

    public async Task<List<SupportedWebsite>> LoadSupportedWebsites()
    {
        try
        {
            string url = r_BaseURL + "/supportedWebsites/getWebsites";
            Debug.WriteLine($"About to reach API: {url}");
            using HttpClient client = new HttpClient();
            var jsonResponse = await client.GetStringAsync(url);
            Debug.WriteLine($"Got JSON response: {jsonResponse}");
        
            List<SupportedWebsite>? supportedWebsites = 
                JsonSerializer.Deserialize<List<SupportedWebsite>>(jsonResponse, rm_CaseInsensitiveOptions);
        
            return supportedWebsites ?? new List<SupportedWebsite>();
        }
        
        catch (Exception e)
        {
            Debug.WriteLine($"Got error: {e.Message}");
            throw;
        }
    }

    public async Task<List<string>> GetAvailableDates(string i_Website)
    {
        string url = r_BaseURL + $"/articlesHistory/getDates?website={i_Website}";
        using HttpClient client = new HttpClient();
        var jsonResponse = await client.GetStringAsync(url);
        List<string>? dates = JsonSerializer.Deserialize<List<string>>(jsonResponse);

        return dates ?? new List<string>();
    }

    public async Task<List<OuterArticle>> GetDatedArticles(string i_Website, string i_Date)
    {
        string url = r_BaseURL + $"/articlesHistory/getArticles?website={i_Website}&date={i_Date}";
        using HttpClient client = new HttpClient();
        var jsonResponse = await client.GetStringAsync(url);
        List<OuterArticle>? articles = JsonSerializer.Deserialize<List<OuterArticle>>(jsonResponse, rm_CaseInsensitiveOptions);
        
        return articles ?? new List<OuterArticle>();
    }
}