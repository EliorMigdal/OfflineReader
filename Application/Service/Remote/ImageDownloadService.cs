namespace Application.Service.Remote;

public class ImageDownloadService
{
    private static ImageDownloadService? m_Instance;
    public static ImageDownloadService Instance
    {
        get
        {
            m_Instance ??= new ImageDownloadService();

            return m_Instance;
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