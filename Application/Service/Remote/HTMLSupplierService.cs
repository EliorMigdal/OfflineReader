namespace Application.Service.Remote;

public class HTMLSupplierService
{
    private static HTMLSupplierService? m_Instance;
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
        using HttpClient client = new HttpClient();
        string htmlCode = await client.GetStringAsync(i_URL);

        return htmlCode;
    }
}