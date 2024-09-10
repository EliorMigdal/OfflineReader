namespace BusinessLogic.AutoDownloadSettings;

public class AutoDownloadSettings
{
    public bool AutoDownloadEnabled { get; set; } = false;
    public int HoursInterval { get; set; } = 24;
    public StartingHour StartHour { get; set; }
    public bool DownloadOnWifiOnly { get; set; } = true;
    public bool AllowCellularRoaming => !DownloadOnWifiOnly;
    public bool DownloadOnlyWhenCharging { get; set; } = true;
    public int MaxArticlesToStoreLocally { get; set; } = 10;

}
