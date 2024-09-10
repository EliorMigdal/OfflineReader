using BusinessLogic.AutoDownloadSettings;
using Application.Service.Local;

namespace Application.ViewModel;

public class AutoDownloadConfigViewModel : BaseViewModel
{
    private bool _isAutoDownloadEnabled;
    private bool _isOnlyOnWifiOptionEnbaled;
    private bool _isOnlyWhenChargingOptionEnbaled;
    private StartingHour _startingHour;
    private string _selectedStartingHourString = string.Empty;
    private string _selectedMaxNumOfArticlesToStoreString = string.Empty;

    private static readonly Dictionary<string, StartingHour> StartingHourMapping = new Dictionary<string, StartingHour>
    {
        { "6 AM", StartingHour.SixAM },
        { "7 AM", StartingHour.SevenAM },
        { "8 AM", StartingHour.EightAM },
        { "9 AM", StartingHour.NineAM },
        { "6 PM", StartingHour.SixPM },
        { "7 PM", StartingHour.SevenPM },
        { "8 PM", StartingHour.EightPM },
        { "9 PM", StartingHour.NinePM }
    };
    private static readonly Dictionary<StartingHour, string> ReverseStartingHourMapping = StartingHourMapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);


    public AutoDownloadSettings AutoDownloadSettings { get; set; }
    
    public AutoDownloadConfigViewModel()
    {
        try
        {
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
        _startingHour = AutoDownloadSettings.StartHour;
        SelectedStartingHourString = ReverseStartingHourMapping.TryGetValue(_startingHour, out var hourString)
                                    ? hourString 
                                    : "6 AM"; // Default value if not found
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
            _startingHour = startingHour;
        }
        else
        {
            _startingHour = StartingHour.SixAM; // Default value
        }    

        AutoDownloadSettings.StartHour = _startingHour;
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
