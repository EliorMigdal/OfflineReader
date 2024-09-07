using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Application.ViewModel;

public class AutoDownloadConfigViewModel : INotifyPropertyChanged
{
    private bool _isAutoDownloadEnabled;
    private int _selectedHour;

    public bool IsAutoDownloadEnabled
    {
        get => _isAutoDownloadEnabled;
        set
        {
            if (_isAutoDownloadEnabled != value)
            {
                _isAutoDownloadEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int SelectedHour
    {
        get => _selectedHour;
        set
        {
            if (_selectedHour != value)
            {
                _selectedHour = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}




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

