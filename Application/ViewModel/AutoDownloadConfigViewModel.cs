using Application.BackgroundTask;
using BusinessLogic.AutoDownloadSettings;
using Application.Service.Local;

namespace Application.ViewModel;

public class AutoDownloadConfigViewModel : BaseViewModel
{
    private bool _isAutoDownloadEnabled;
    private bool _isOnlyOnWifiOptionEnbaled;
    private bool _isOnlyWhenChargingOptionEnbaled;
    private eStartingHour _eStartingHour;
    private string _selectedStartingHourString = string.Empty;
    private eDownloadInterval _eDownloadInterval;
    private string _selectedDownloadIntervalString = string.Empty;
    private string _selectedMaxNumOfArticlesToStoreString = string.Empty;

    private static readonly Dictionary<string, eStartingHour> StartingHourMapping = new Dictionary<string, eStartingHour>
    {
        { "6 AM", eStartingHour.SixAM },
        { "7 AM", eStartingHour.SevenAM },
        { "8 AM", eStartingHour.EightAM },
        { "9 AM", eStartingHour.NineAM },
        { "6 PM", eStartingHour.SixPM },
        { "7 PM", eStartingHour.SevenPM },
        { "8 PM", eStartingHour.EightPM },
        { "9 PM", eStartingHour.NinePM }
    };
    private static readonly Dictionary<eStartingHour, string> ReverseStartingHourMapping = StartingHourMapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    private static readonly Dictionary<string, eDownloadInterval> DownloadIntervalMapping = new Dictionary<string, eDownloadInterval>
    {
        { "Every 6 Hours", eDownloadInterval.Every6Hours },
        { "Every 12 Hours", eDownloadInterval.Every12Hours },
        { "Every 24 Hours", eDownloadInterval.Every24Hours },
        { "Every 2 Days", eDownloadInterval.Every2Days },
        { "Every 3 Days", eDownloadInterval.Every3Days },
        { "Every 4 Days", eDownloadInterval.Every4Days },
        { "Every 5 Days", eDownloadInterval.Every5Days },
        { "Every 1 Week", eDownloadInterval.Every1Week },
        { "Every 2 Weeks", eDownloadInterval.Every2Weeks }
    };
    private static readonly Dictionary<eDownloadInterval, string> ReverseDownloadIntervalMapping = DownloadIntervalMapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    private IBackgroundTaskService m_BackgroundTaskService;


    public AutoDownloadSettings AutoDownloadSettings { get; set; }
    
    public AutoDownloadConfigViewModel(IBackgroundTaskService i_Service)
    {
        try
        {
            m_BackgroundTaskService = i_Service;
            AutoDownloadSettings = ConfigService.GetSettings();
        }
        
        catch (Exception e)
        {
            AutoDownloadSettings = new AutoDownloadSettings();
        }
        
        InitializeProperties();
    }

    private void InitializeProperties()
    {
        IsAutoDownloadEnabled = AutoDownloadSettings.AutoDownloadEnabled;
        IsOnlyOnWifiOptionEnbaled = AutoDownloadSettings.DownloadOnWifiOnly;
        IsOnlyWhenChargingOptionEnbaled = AutoDownloadSettings.DownloadOnlyWhenCharging;
        _eStartingHour = AutoDownloadSettings.StartHour;
        SelectedStartingHourString = ReverseStartingHourMapping.TryGetValue(_eStartingHour, out var hourString)
                                    ? hourString 
                                    : "6 AM"; // Default value if not found
        _eDownloadInterval = AutoDownloadSettings.DownloadInterval;
        SelectedDownloadIntervalString = ReverseDownloadIntervalMapping.TryGetValue(_eDownloadInterval, out var intervalString)
                                        ? intervalString
                                        : "Every 24 Hours"; // Default value if not found
        SelectedMaxNumOfArticlesToStoreString = AutoDownloadSettings.MaxArticlesToStoreLocally.ToString();
    }

    public bool IsAutoDownloadEnabled
    {
        get => _isAutoDownloadEnabled;
        set
        {
            if (_isAutoDownloadEnabled != value)
            {
                _isAutoDownloadEnabled = AutoDownloadSettings.AutoDownloadEnabled = value;
                SaveAutoDownloadSettingsToConfigFile();
                OnPropertyChanged();
            }
        }
    }

    public bool IsOnlyOnWifiOptionEnbaled
    {
        get => _isOnlyOnWifiOptionEnbaled;
        set
        {
            if (_isOnlyOnWifiOptionEnbaled != value)
            {
                _isOnlyOnWifiOptionEnbaled = AutoDownloadSettings.DownloadOnWifiOnly = value;
                SaveAutoDownloadSettingsToConfigFile();
                OnPropertyChanged();
            }
        }
    }

    public bool IsOnlyWhenChargingOptionEnbaled
    {
        get => _isOnlyWhenChargingOptionEnbaled;
        set
        {
            if (_isOnlyWhenChargingOptionEnbaled != value)
            {
                _isOnlyWhenChargingOptionEnbaled = AutoDownloadSettings.DownloadOnlyWhenCharging = value;
                SaveAutoDownloadSettingsToConfigFile();
                OnPropertyChanged();
            }
        }
    }

    public string SelectedStartingHourString
    {
        get => _selectedStartingHourString;
        set
        {
            if (_selectedStartingHourString != value)
            {
                _selectedStartingHourString = value;
                UpdateStartingHourInSettings();
                SaveAutoDownloadSettingsToConfigFile();
                OnPropertyChanged();
            }
        }
    }

    private void UpdateStartingHourInSettings()
    {
        if (StartingHourMapping.TryGetValue(SelectedStartingHourString, out var startingHour))
        {
            _eStartingHour = startingHour;
        }
        else
        {
            _eStartingHour = eStartingHour.SixAM; // Default value
        }    

        AutoDownloadSettings.StartHour = _eStartingHour;
    }

    public string SelectedDownloadIntervalString
    {
        get => _selectedDownloadIntervalString;
        set
        {
            if (_selectedDownloadIntervalString != value)
            {
                _selectedDownloadIntervalString = value;
                UpdateDownloadIntervalInSettings();
                SaveAutoDownloadSettingsToConfigFile();
                OnPropertyChanged();
            }
        }
    }

    private void UpdateDownloadIntervalInSettings()
    {
        if (DownloadIntervalMapping.TryGetValue(SelectedDownloadIntervalString, out var downloadInterval))
        {
            _eDownloadInterval = downloadInterval;
        }
        else
        {
            _eDownloadInterval = eDownloadInterval.Every24Hours; // Default value
        }

        AutoDownloadSettings.DownloadInterval = _eDownloadInterval;
    }

    public string SelectedMaxNumOfArticlesToStoreString
    {
        get => _selectedMaxNumOfArticlesToStoreString;
        set
        {
            if (_selectedMaxNumOfArticlesToStoreString != value)
            {
                _selectedMaxNumOfArticlesToStoreString = value;
                UpdateMaxNumOfArticlesToStoreInSettings();
                SaveAutoDownloadSettingsToConfigFile();
                OnPropertyChanged();
            }
        }
    }

    private void UpdateMaxNumOfArticlesToStoreInSettings()
    {
        AutoDownloadSettings.MaxArticlesToStoreLocally = int.Parse(SelectedMaxNumOfArticlesToStoreString);
    }

    private void SaveAutoDownloadSettingsToConfigFile()
    {
        ConfigService.SaveSettings(AutoDownloadSettings);
    }
}
