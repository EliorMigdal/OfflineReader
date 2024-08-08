namespace OfflineReader.Service.Remote;

public class HTMLSupplierService
{
    private HttpClient Client { get; } = HTTPClient.Instance;
    private static HTMLSupplierService m_Instance;
    public static HTMLSupplierService Instance
    {
        get
        {
            m_Instance ??= new HTMLSupplierService();

            return m_Instance;
        }
    }
    
    private HTMLSupplierService() {}

    public async Task<string> GetHTMLAsync(string i_URL)
    {
        string htmlCode = await downloadWebPageAsync(i_URL);

        return htmlCode;
    }

    private async Task<string> downloadWebPageAsync(string i_URL)
    {
        string htmlCode = await Client.GetStringAsync(i_URL);

        return htmlCode;
    }
}