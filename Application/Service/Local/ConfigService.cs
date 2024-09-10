using Application.Service.Remote;
using BusinessLogic.AutoDownloadSettings;
using BusinessLogic.SupportedWebsite;
using System.Text.Json;

namespace Application.Service.Local;

public sealed class ConfigService
{
    private static string ConfigFilePath => Path.Combine(FileSystem.AppDataDirectory, "Config");
    private readonly ServerAPI r_ServerAPI = ServerAPI.Instance;
    private static readonly object rm_CreationLock = new();
    private static ConfigService? m_Instance;
    public static ConfigService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ConfigService();

                return m_Instance;
            }
        }
    }

    private ConfigService()
    {
        if (!Directory.Exists(ConfigFilePath))
        {
            Directory.CreateDirectory(ConfigFilePath);
        }
    }

    public static AutoDownloadSettings GetSettings()
    {
        AutoDownloadSettings settings;
        
        try
        {
            using Stream fileStream = new FileStream(ConfigFilePath + "/settings.json", FileMode.OpenOrCreate);
            settings = JsonSerializer.Deserialize<AutoDownloadSettings>(fileStream) ?? new AutoDownloadSettings();
        }
        
        catch (Exception)
        {
            settings = new AutoDownloadSettings();
        }
        
        return settings;
    }

    public static List<SupportedWebsite> GetSelectedWebsites()
    {
        List<SupportedWebsite> websites;
        
        try
        {
            using Stream fileStream = new FileStream(ConfigFilePath + "/websites.json", FileMode.OpenOrCreate);
            websites = JsonSerializer.Deserialize<List<SupportedWebsite>>(fileStream) ?? new List<SupportedWebsite>();
        }
        
        catch (Exception)
        {
            websites = new List<SupportedWebsite>();
        }


        return websites;
    }

    public static void SaveSelectedWebsites(List<SupportedWebsite> i_Websites)
    {
        using Stream fileStream = new FileStream(ConfigFilePath + "/websites.json", FileMode.Create);
        JsonSerializer.Serialize(fileStream, i_Websites);
    }

    public static void SaveSettings(AutoDownloadSettings i_Settings)
    {
        using Stream fileStream = new FileStream(ConfigFilePath + "/settings.json", FileMode.Create);
        JsonSerializer.Serialize(fileStream, i_Settings);
    }

    public async Task<List<SupportedWebsite>> LoadSupportedWebsites()
    {
        return await r_ServerAPI.LoadSupportedWebsites();
    }
}