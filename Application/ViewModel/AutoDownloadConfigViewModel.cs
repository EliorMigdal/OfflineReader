using BusinessLogic.AutoDownloadSettings;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application.View;
using Application.Service.Local;
using System.Windows.Input;

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


    //public ICommand SaveAutoDownloadSettingsCommand { get; private set; }


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
        //SaveAutoDownloadSettingsCommand = new Command(onSaveAutoDownloadSettingsCommand);
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
        SaveAutoDownloadSettingsToConfigFile();
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

    //private void onSaveAutoDownloadSettingsCommand()
    //{
    //    if (IsBusy)
    //        return;

    //    IsBusy = true;
    //    try
    //    {
    //        ConfigService.SaveSettings(AutoDownloadSettings);
    //    }
    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}
}


//switch (SelectedStartingHourString)
//{
//    case "6 AM":
//        _startingHour = StartingHour.SixAM;
//        break;
//    case "7 AM":
//        _startingHour = StartingHour.SevenAM;
//        break;
//    case "8 AM":
//        _startingHour = StartingHour.EightAM;
//        break;
//    case "9 AM":
//        _startingHour = StartingHour.NineAM;
//        break;
//    case "6 PM":
//        _startingHour = StartingHour.SixPM;
//        break;
//    case "7 PM":
//        _startingHour = StartingHour.SevenPM;
//        break;
//    case "8 PM":
//        _startingHour = StartingHour.EightPM;
//        break;
//    case "9 PM":
//        _startingHour = StartingHour.NinePM;
//        break;
//    default:
//        _startingHour = StartingHour.SixAM;
//        break;
//};


//public event PropertyChangedEventHandler? PropertyChanged;

//protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
//{
//    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//}



//private string _selectedTimeLapseString = string.Empty;

//public string SelectedTimeLapseString
//{
//    get => _selectedTimeLapseString;
//    set
//    {
//        if (_selectedTimeLapseString != value)
//        {
//            _selectedTimeLapseString = value;
//            OnPropertyChanged();
//            OnPropertyChanged(nameof(SelectedTimeInterval));
//            // UpdateTimeLapse();
//            //UpdateDownloadInterval();
//        }
//    }
//}

//private void UpdateDownloadInterval()
//{
//    DownloadInterval downloadInterval; 

//    switch (SelectedTimeLapseString)
//    {
//        case "Every 6 hours":
//            downloadInterval = DownloadInterval.Every6Hours;
//            break;
//        case "Every 12 hours":
//            downloadInterval = DownloadInterval.Every12Hours;
//            break;
//        case "Every 24 hours":
//            downloadInterval = DownloadInterval.Every24Hours;
//            break;
//        case "Every 2 days":
//            downloadInterval = DownloadInterval.Every2Days;
//            break;
//        case "Every 3 days":
//            downloadInterval = DownloadInterval.Every3Days;
//            break;


//        default:
//            DownloadInterval.Every6Hours;
//            break;
//    };

////    AutoDownloadSettings.Interval = SelectedTimeInterval switch
////    {
////        1 => DownloadInterval.EveryHour,
////        24 => DownloadInterval.EveryDay,
////        168 => DownloadInterval.EveryWeek,
////        _ => DownloadInterval.Every6Hours
////    };
//    int timeLapse = SelectedTimeInterval;
//}

//public int SelectedTimeInterval
//{
//    get
//    {
//        if (SelectedTimeLapseString.Contains("hour"))
//        {
//            return int.Parse(SelectedTimeLapseString.Split(' ')[1]);
//        }
//        else if (SelectedTimeLapseString.Contains("day"))
//        {
//            return int.Parse(SelectedTimeLapseString.Split(' ')[1]) * 24;
//        }
//        else if (SelectedTimeLapseString.Contains("week"))
//        {
//            return int.Parse(SelectedTimeLapseString.Split(' ')[1]) * 24 * 7;
//        }
//        return 0;
//    }
//}

//public void UpdateTimeLapse()
//{
//    int timeLapse = SelectedTimeInterval;
//    // Use the timeLapse value for some logic
//    Console.WriteLine($"Time Lapse in hours: {timeLapse}");
//}


//public class AutoDownloadConfigViewModel : INotifyPropertyChanged
//    {
//        private bool _isAutoDownloadEnabled;
//        private int _selectedHour;

//        public AutoDownloadConfigViewModel()
//        {
//            HoursList = Enumerable.Range(1, 24).ToList(); // Populate with 1-24
//        }

//        public List<int> HoursList { get; }

//        public bool IsAutoDownloadEnabled
//        {
//            get => _isAutoDownloadEnabled;
//            set
//            {
//                _isAutoDownloadEnabled = value;
//                OnPropertyChanged();
//            }
//        }

//        public int SelectedHour
//        {
//            get => _selectedHour;
//            set
//            {
//                _selectedHour = value;
//                OnPropertyChanged();
//            }
//        }

//        public event PropertyChangedEventHandler PropertyChanged;

//        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }

//using Application.ViewModel;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Runtime.CompilerServices;
//using System.Windows.Input;

//namespace Application.ViewModel;

//public class AutoDownloadConfigViewModel : BaseViewModel, INotifyPropertyChanged
//{
//    private string? _selectedTimeUnit;
//    private bool _isAutoDownloadEnabled;
//    private bool _isOnlyOnWifiOptionEnbaled;
//    private int _maxNumOfArticlesToSave;
//    private int _selectedHour;
//    private int _selectedDay;
//    private int _selectedWeek;

//    public AutoDownloadConfigViewModel()
//    {
//        TimeUnits = new List<string> { "Hours", "Days", "Weeks" };
//        HoursList = Enumerable.Range(1, 24).ToList(); // Populate with 1-24
//        DaysList = Enumerable.Range(1, 31).ToList(); // Populate with 1-31
//        WeeksList = Enumerable.Range(1, 10).ToList(); // Populate with 1-10
//        PossibleMaxNumOfArticlesToSave = Enumerable.Range(1, 10).Select(x => x * 10).ToList(); // Populate as needed
//        SaveTimeLapseCommand = new Command(OnSaveTimeLapse);
//    }

//    public List<string> TimeUnits { get; }
//    public List<int> HoursList { get; }
//    public List<int> DaysList { get; }
//    public List<int> WeeksList { get; }
//    public List<int> PossibleMaxNumOfArticlesToSave { get; }

//    public string SelectedTimeUnit
//    {
//        get => _selectedTimeUnit;
//        set
//        {
//            _selectedTimeUnit = value;
//            OnPropertyChanged();
//            OnPropertyChanged(nameof(IsHoursSelected));
//            OnPropertyChanged(nameof(IsDaysSelected));
//            OnPropertyChanged(nameof(IsWeeksSelected));
//        }
//    }

//    public bool IsAutoDownloadEnabled
//    {
//        get => _isAutoDownloadEnabled;
//        set
//        {
//            _isAutoDownloadEnabled = value;
//            OnPropertyChanged();
//        }
//    }

//    public bool IsOnlyOnWifiOptionEnbaled
//    {
//        get => _isOnlyOnWifiOptionEnbaled;
//        set
//        {
//            _isOnlyOnWifiOptionEnbaled = value;
//            OnPropertyChanged();
//        }
//    }

//    public int MaxNumOfArticlesToSave
//    {
//        get => _maxNumOfArticlesToSave;
//        set
//        {
//            _maxNumOfArticlesToSave = value;
//            OnPropertyChanged();
//        }
//    }

//    public int SelectedHour
//    {
//        get => _selectedHour;
//        set
//        {
//            _selectedHour = value;
//            OnPropertyChanged();
//        }
//    }

//    public int SelectedDay
//    {
//        get => _selectedDay;
//        set
//        {
//            _selectedDay = value;
//            OnPropertyChanged();
//        }
//    }

//    public int SelectedWeek
//    {
//        get => _selectedWeek;
//        set
//        {
//            _selectedWeek = value;
//            OnPropertyChanged();
//        }
//    }

//    public bool IsHoursSelected => SelectedTimeUnit == "Hours";
//    public bool IsDaysSelected => SelectedTimeUnit == "Days";
//    public bool IsWeeksSelected => SelectedTimeUnit == "Weeks";

//    public ICommand SaveTimeLapseCommand { get; }

//    private void OnSaveTimeLapse()
//    {
//        // Implement logic to save the time lapse configuration
//    }

//    public event PropertyChangedEventHandler? PropertyChanged;

//    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
//    {
//        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}




////using Application.View;
////using System.Collections.ObjectModel;
////using System.Windows.Input;

////namespace Application.ViewModel;

////public class AutoDownloadConfigViewModel : BaseViewModel
////{
////    //public class TimeLapse
////    //{
////    //    public int NumOfHours { get; set; } = 0;
////    //    public int NumOfDays { get; set; } = 0;
////    //    public int NumOfWeeks { get; set; } = 0;
////    //}
////    //public bool IsTimeLapseEnabled { get; set; } = false;

////    //public ObservableCollection<TimeLapse> TimeLapseList { get; private set; }

////    public class Hours
////    {
////        public int NumOfHoursUserSelected { get; set; } = 0;

////        public List<int> HoursList { get; set; } = new List<int>();

////        public Hours()
////        {
////            for (int i = 1; i <= 24; i++)
////            {
////                HoursList.Add(i);
////            }
////        }
////    }

////    public class Days
////    {
////        public int NumOfDaysUserSelected { get; set; } = 0;

////        public List<int> DaysList { get; set; } = new List<int>();

////        public Days()
////        {
////            for (int i = 1; i <= 31; i++)
////            {
////                DaysList.Add(i);
////            }
////        }
////    }

////    public class Weeks
////    {
////        public int NumOfWeeksUserSelected { get; set; } = 0;

////        public List<int> WeeksList { get; set; } = new List<int>();

////        public Weeks()
////        {
////            for (int i = 1; i <= 10; i++)
////            {
////                WeeksList.Add(i);
////            }
////        }
////    }


////    public bool IsAutoDownloadEnabled { get; set; } = false;

////    public bool IsOnlyOnWifiOptionEnbaled { get; set; } = false;

////    public bool IsRoamingAndWifiOptionEnabled => !IsOnlyOnWifiOptionEnbaled;

////    public int MaxNumOfArticlesToSave { get; set; }

////    public List<int> PossibleMaxNumOfArticlesToSave { get; set; } = new List<int>();

////    public ICommand EnableOrDisableAutoDownload { get; set; }

////    public ICommand EnableOrDisableOnlyOnWifiOption { get; set; }

////    public ICommand SaveTimeLapseCommand { get; private set; }

////    public AutoDownloadConfigViewModel()
////    {
////        //TimeLapseList = new ObservableCollection<TimeLapse>();
////        Hours numOfHours = new Hours();
////        Days numOfDays = new Days();
////        Weeks numOfWeeks = new Weeks();
////        initializePossibleMaxNumOfArticlesToSave();
////        SaveTimeLapseCommand = new Command(OnSaveTimeLapse);
////        EnableOrDisableAutoDownload = new Command(OnEnableOrDisableAutoDownload);
////        EnableOrDisableOnlyOnWifiOption = new Command(OnEnableOrDisableOnlyOnWifiOption);
////    }

////    private void OnEnableOrDisableOnlyOnWifiOption(object obj)
////    {
////        throw new NotImplementedException();
////    }

////    private void OnEnableOrDisableAutoDownload()
////    {

////    }

////    private void initializePossibleMaxNumOfArticlesToSave()
////    {
////        int i = MaxNumOfArticlesToSave = 10;

////        while (i <= 100)
////        {
////            PossibleMaxNumOfArticlesToSave.Add(i);
////            i += 10;
////        }
////    }

////    private void OnSaveTimeLapse()
////    {

////    }


////}

