using System.Diagnostics;
using BusinessLogic.SupportedWebsite;

namespace Application.Service.Remote;

public sealed class ServerAPI
{
    private readonly string r_BaseURL = "http://localhost:5119/offlineReader";
    private static readonly object rm_CreationLock = new();
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