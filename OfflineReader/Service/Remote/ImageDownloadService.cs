namespace OfflineReader.Service.Remote;

public class ImageDownloadService
{
    private HttpClient Client { get; } = HTTPClient.Instance;
    private static ImageDownloadService m_Instance;
    public static ImageDownloadService Instance
    {
        get
        {
            m_Instance ??= new ImageDownloadService();

            return m_Instance;
        }
    }

    public async Task DownloadImageAsync(string imageUrl, string localPath)
    {
        byte[] imageBytes = await Client.GetByteArrayAsync(imageUrl);
        await File.WriteAllBytesAsync(localPath, imageBytes);
    }
}