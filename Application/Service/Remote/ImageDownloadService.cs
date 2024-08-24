namespace Application.Service.Remote;

public sealed class ImageDownloadService
{
    private static readonly object rm_CreationLock = new();
    private static ImageDownloadService? m_Instance;
    public static ImageDownloadService Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ImageDownloadService();

                return m_Instance;
            }
        }
    }
    
    private ImageDownloadService() {}

    public async Task DownloadImageAsync(string imageUrl, string localPath)
    {
        using HttpClient client = new HttpClient();
        byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
        await File.WriteAllBytesAsync(localPath, imageBytes);
    }
}