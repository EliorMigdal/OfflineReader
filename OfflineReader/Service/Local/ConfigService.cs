using System.Diagnostics;
using Newtonsoft.Json;
using OfflineReader.Model;

namespace OfflineReader.Service.Local;

public class ConfigService
{
    public static string ConfigFilePath => Path.Combine(FileSystem.AppDataDirectory, "Config.json");
    private static ConfigService m_Instance;
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

    public static List<string> LoadSupportedWebsites()
    {
        if (File.Exists(ConfigFilePath))
        {
            string json = File.ReadAllText(ConfigFilePath);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }

        else
        {
            return new List<string>();
        }
    }
}