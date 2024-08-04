namespace OfflineReader.Service.Remote;

public class HTTPClient
{
    private static HttpClient m_Instance;
    public static HttpClient Instance
    {
        get
        {
            m_Instance ??= new HttpClient();

            return m_Instance;
        }
    }

    private HTTPClient() { }
}