using Application.View;
using System.Windows.Input;
using BusinessLogic.SupportedWebsite;
using Application.Service.Remote;
using Application.Service.Local;
using AsyncAwaitBestPractices.MVVM;
using System.Collections.ObjectModel;
using BusinessLogic.AutoDownloadSettings;

namespace Application.ViewModel;

public class ChooseSitesViewModel : BaseViewModel
{
    //public class SinglePossiblyChosenSite
    //{
    //    private bool _isSelected = false;
    //    public SupportedWebsite SupportedWebsite { get; set; } = null!;
    //    public string Name => SupportedWebsite.Name;
    //    public bool IsSelected 
    //    { 
    //        get => _isSelected;
    //        set
    //        {
    //            if (_isSelected != value)
    //            {
    //                _isSelected = value;
    //                SaveAutoDownloadSettingsToConfigFile();
    //            }
    //        }
    //    }
    //    public SinglePossiblyChosenSite(SupportedWebsite supportedWebsite, bool isSelected)
    //    {
    //        SupportedWebsite = supportedWebsite;
    //        IsSelected = isSelected;
    //    }
    //}
    //public static ObservableCollection<SinglePossiblyChosenSite> AllPossibleSitesToChooseFrom { get; private set; } = new();

    public class Site
    {
        public string Name { get; set; } = string.Empty;

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                }
            }
        }
        //public bool IsSelected { get; set; } = false;
        public SupportedWebsite SupportedWebsite { get; set; } = null!;
    }

    private bool _showMessageThatSaveCompleted = false;
    public bool ShowMessageThatSaveCompleted
    {
        get => _showMessageThatSaveCompleted;
        set
        {
            _showMessageThatSaveCompleted = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Site> Sites { get; private set; } = new();
    //public List<(SupportedWebsite supportedSite, bool isSelected)> AllPossibleSites { get; private set; } = new();
    private readonly ServerAPI rm_ServerAPI = ServerAPI.Instance;


    public ICommand SaveSitesCommand { get; private set; }

    public ChooseSitesViewModel()
    {
        List<SupportedWebsite> selectedWebsites;
        try
        {
            selectedWebsites = ConfigService.GetSelectedWebsites();
        } 
        catch (Exception e)
        {
            selectedWebsites = new List<SupportedWebsite>();
        }
        initializeSupportedWebsites(selectedWebsites);
        //SetNamesOfSites();
        SaveSitesCommand = new AsyncCommand(onSaveSitesCommand);
    }

    private async void initializeSupportedWebsites(List<SupportedWebsite> selectedWebsites)
    {
        try
        {
            List<string> selectedWebsitesNames = new List<string>();
            foreach (SupportedWebsite supportedWebsite in selectedWebsites)
            {
                selectedWebsitesNames.Add(supportedWebsite.Name);
            }

            List<SupportedWebsite> supportedWebsites = await rm_ServerAPI.LoadSupportedWebsites();


            foreach (SupportedWebsite supportedWebsite in supportedWebsites)
            {
                if (selectedWebsitesNames.Contains(supportedWebsite.Name))
                {
                    //AllPossibleSitesToChooseFrom.Add(new SinglePossiblyChosenSite(supportedWebsite, true));
                    Sites.Add(new Site { Name = supportedWebsite.Name, IsSelected = true, SupportedWebsite = supportedWebsite });
                }
                else
                {
                    //AllPossibleSitesToChooseFrom.Add(new SinglePossiblyChosenSite(supportedWebsite, false));
                    Sites.Add(new Site { Name = supportedWebsite.Name, IsSelected = false, SupportedWebsite = supportedWebsite });
                }
            }
        }

        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Error",
                "Fetching websites has failed.", "OK");
        }
    }

    //internal static void SaveAutoDownloadSettingsToConfigFile()
    //{
    //    List<SupportedWebsite> chosenSites = new List<SupportedWebsite>();
    //    foreach (var site in Sites)// AllPossibleSitesToChooseFrom)
    //    {
    //        if (site.IsSelected)
    //        {
    //            chosenSites.Add(site.SupportedWebsite);
    //        }
    //    }
    //    ConfigService.SaveSelectedWebsites(chosenSites);
    //}

    //private void SetNamesOfSites()
    //{
    //    Sites.Add(new Site { Name = "Mako", IsSelected = false });
    //    Sites.Add(new Site { Name = "Ynet", IsSelected = false });
    //    Sites.Add(new Site { Name = "TheMarker", IsSelected = false });
    //    Sites.Add(new Site { Name = "Walla", IsSelected = false });
    //    Sites.Add(new Site { Name = "CNN", IsSelected = false });
    //    Sites.Add(new Site { Name = "BBC", IsSelected = false });
    //}

    private async Task onSaveSitesCommand()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            List<SupportedWebsite> chosenSites = new List<SupportedWebsite>();
            foreach (var site in Sites)
            {
                if (site.IsSelected)
                {
                    chosenSites.Add(site.SupportedWebsite);
                }
            }

            ConfigService.SaveSelectedWebsites(chosenSites);
            
            ShowMessageThatSaveCompleted = true;
            await Task.Delay(5000);
            ShowMessageThatSaveCompleted = false;
        }
        finally
        {
            IsBusy = false;
        }
    }


    //private void OnSaveSitesCommand()
    //{



    //    //var selectedSites = AllPossibleSites.Where(site => site.IsSelected).ToList();

    //    //// Logic to handle selected sites
    //    //foreach (var site in selectedSites)
    //    //{
    //    //}
    //}
}
