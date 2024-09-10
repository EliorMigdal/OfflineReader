namespace Application.Helpers;

public sealed class ConnectivityManager
{
    private static readonly object rm_CreationLock = new();
    private static ConnectivityManager? m_Instance;
    public static ConnectivityManager Instance
    {
        get
        {
            if (m_Instance is not null) return m_Instance;

            lock (rm_CreationLock)
            {
                m_Instance ??= new ConnectivityManager();

                return m_Instance;
            }
        }
    }
    
    private readonly IConnectivity m_Connectivity = Connectivity.Current;
    
    private ConnectivityManager() {}

    public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged
    {
        add => m_Connectivity.ConnectivityChanged += value;
        remove => m_Connectivity.ConnectivityChanged -= value;
    }
    
    public bool IsDeviceConnected()
    {
        return m_Connectivity.NetworkAccess == NetworkAccess.Internet;
    }

    public bool IsDeviceConnectedToWiFi()
    {
        return m_Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi);
    }
    
    public async Task AlertConnectivityIssue()
    {
        await Shell.Current.DisplayAlert("No connectivity!",
            "Please check internet connection and try again.", "OK");
    }
}