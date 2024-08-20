using System.Diagnostics;
using BusinessLogic.SupportedWebsite;

namespace Application.Service.Remote;

public class ServerAPI
{
    private readonly string r_BaseURL = "http://localhost:5000/offlineReader";
    private static ServerAPI? m_Instance;
    public static ServerAPI Instance
    {
        get
        {
            m_Instance ??= new ServerAPI();

            return m_Instance;
        }
    }

    public async Task<List<SupportedWebsite>> LoadSupportedWebsites()
    {
        string url = r_BaseURL + "/supportedWebsites/getWebsites";
        using HttpClient client = new HttpClient();
        var jsonResponse = await client.GetStringAsync(url);

        var options = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        List<SupportedWebsite>? supportedWebsites = 
            System.Text.Json.JsonSerializer.Deserialize<List<SupportedWebsite>>(jsonResponse, options);
        
        Debug.WriteLine($"Got JSON: {jsonResponse}");
        
        return supportedWebsites ?? new List<SupportedWebsite>();
    }
}