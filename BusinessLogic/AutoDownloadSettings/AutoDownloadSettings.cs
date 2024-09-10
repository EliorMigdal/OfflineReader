using BusinessLogic.SupportedWebsite;

namespace BusinessLogic.AutoDownloadSettings;

public class AutoDownloadSettings
{
    public bool AutoDownloadEnabled { get; set; } = false;
    //public DownloadInterval Interval { get; set; }
    public int HoursInterval { get; set; } = 24;
    public StartingHour StartHour { get; set; }
    public bool DownloadOnWifiOnly { get; set; } = true;
    public bool AllowCellularRoaming => !DownloadOnWifiOnly;
    public bool DownloadOnlyWhenCharging { get; set; } = true;
    public int MaxArticlesToStoreLocally { get; set; } = 10;
    //public List<SupportedWebsite.SupportedWebsite> SelectedSites { get; set; } = new();

}
