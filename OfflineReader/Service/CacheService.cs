namespace OfflineReader.Service;

public class CacheService
{
    public static string CachePath => Path.Combine(FileSystem.AppDataDirectory, "/Cache");


}