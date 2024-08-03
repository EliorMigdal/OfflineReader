using System.Diagnostics;

namespace OfflineReader.Service;

public class ImageDownloadService
{
    private HttpClient Client { get; set; } = new HttpClient();

    public async Task DownloadImageAsync(string imageUrl, string localPath)
    {
        Debug.WriteLine($"Downloading image from {imageUrl} to {localPath}");
        byte[] imageBytes = await Client.GetByteArrayAsync(imageUrl);
        Debug.WriteLine($"Downloaded image from {imageUrl}");
        await File.WriteAllBytesAsync(localPath, imageBytes);
        Debug.WriteLine($"Saved image to {localPath}");
    }
}