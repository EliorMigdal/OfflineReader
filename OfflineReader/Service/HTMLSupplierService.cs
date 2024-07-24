using OfflineReader.Model;

namespace OfflineReader.Service;

public class HTMLSupplierService
{
    private PathParsingService PathParser { get; set; } = new();

    public async Task<string> GetHTMLAsync(string i_URL)
    {
        //string htmlCode = await findHTMLLocallyAsync(i_Article);

        //if (string.IsNullOrEmpty(htmlCode))
        //{
        //    htmlCode = await downloadWebPageAsync(i_Article.URL);
        //}

        string htmlCode = await downloadWebPageAsync(i_URL);

        return htmlCode;
    }

    private async Task<string> findHTMLLocallyAsync(Article i_Article)
    {
        string path = PathParser.ParseArticleToPath(i_Article);
        string htmlCode = string.Empty;

        if (File.Exists(path))
        {
            htmlCode = await Task.Run(() => File.ReadAllText(path));
        }

        return null;
    }

    private async Task<string> downloadWebPageAsync(string i_URL)
    {
        using HttpClient client = new();
        string htmlCode = await client.GetStringAsync(i_URL);
        return htmlCode;
    }
}