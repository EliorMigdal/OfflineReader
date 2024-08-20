using System.Diagnostics;
using Application.Service.Remote;
using BusinessLogic.SupportedWebsite;
using Newtonsoft.Json;

namespace Application.Service.Local;

public class ConfigService
{
    private static string ConfigFilePath => Path.Combine(FileSystem.AppDataDirectory, "Config.json");
    private readonly ServerAPI r_ServerAPI = ServerAPI.Instance;
    private static ConfigService? m_Instance;
    public static ConfigService Instance
    {
        get
        {
            m_Instance ??= new ConfigService();

            return m_Instance;
        }
    }
    
    private ConfigService() {}

    public static bool DoesConfigFileExist()
    {
        return File.Exists(ConfigFilePath);
    }

    public static string GetConfigFilePath()
    {
        return ConfigFilePath;
    }

    public static void AddWebsitesToConfigFile(List<SupportedWebsite> i_SupportedWebsites)
    {
        try
        {
            List<string> urls = i_SupportedWebsites.Select(website => website.URL).ToList();
            string json = JsonConvert.SerializeObject(urls, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, json);
            Debug.WriteLine($"Created config file!");
        }

        catch (Exception error)
        {
            Debug.WriteLine($"Failed to create config file! Error: {error}");
        }

    }

    public async Task<List<SupportedWebsite>> LoadSupportedWebsites()
    {
        return await r_ServerAPI.LoadSupportedWebsites();
    }
}