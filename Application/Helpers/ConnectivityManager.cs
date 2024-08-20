namespace Application.Helpers;

public class ConnectivityManager
{
    private static ConnectivityManager? m_Instance;
    public static ConnectivityManager Instance
    {
        get
        {
            m_Instance ??= new ConnectivityManager();

            return m_Instance;
        }
    }
    private readonly IConnectivity m_Connectivity = Connectivity.Current;
    public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged
    {
        add => m_Connectivity.ConnectivityChanged += value;
        remove => m_Connectivity.ConnectivityChanged -= value;
    }
    
    private ConnectivityManager() {}

    public bool IsDeviceConnected()
    {
        return m_Connectivity.NetworkAccess == NetworkAccess.Internet;
    }

    public bool IsDeviceConnectedToWiFi()
    {
        throw new NotImplementedException();
    }
    
    public async Task AlertConnectivityIssue()
    {
        await Shell.Current.DisplayAlert("No connectivity!",
            "Please check internet connection and try again.", "OK");
    }
}