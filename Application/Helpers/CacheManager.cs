namespace Application.Helpers;

public sealed class CacheManager
{
    private static readonly object sr_CreationLock = new();
    private static CacheManager? s_Instance;
    public static CacheManager Instance
    {
        get
        {
            if (s_Instance is not null) return s_Instance;

            lock (sr_CreationLock)
            {
                s_Instance ??= new CacheManager();

                return s_Instance;
            }
        }
    }
    private CacheManager() {}
    
    
}