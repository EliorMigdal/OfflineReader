using System.Collections.ObjectModel;
using System.Windows.Input;
using Application.Service.Remote;
using Application.View;
using AsyncAwaitBestPractices.MVVM;
using BusinessLogic.SupportedWebsite;

namespace Application.ViewModel;

public class HistoryViewModel : BaseViewModel
{
    public ObservableCollection<SupportedWebsite> SupportedWebsites { get; } = new();
    public SupportedWebsite? SelectedWebsite { get; set; }
    public ICommand SelectedWebsiteCommand { get; set; }
    private readonly ServerAPI rm_ServerAPI = ServerAPI.Instance;

    public HistoryViewModel()
    {
        initializeSupportedWebsites();
        SelectedWebsiteCommand = new AsyncCommand(onWebsiteSelection);
    }

    private async void initializeSupportedWebsites()
    {
        try
        {
            List<SupportedWebsite> supportedWebsites = await rm_ServerAPI.LoadSupportedWebsites();

            foreach (SupportedWebsite supportedWebsite in supportedWebsites)
            {
                SupportedWebsites.Add(supportedWebsite);
            }
        }
        
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Error",
                "Fetching websites has failed.", "OK");
        }
    }

    private async Task onWebsiteSelection()
    {
        try
        {
            if (SelectedWebsite != null)
            {
                List<string> dates = await rm_ServerAPI.GetAvailableDates(SelectedWebsite.Name);
            
                await Shell.Current.GoToAsync(nameof(DatePickerPage), true, new Dictionary<string, object>
                {
                    { "Dates", dates }, {"Website", SelectedWebsite.Name}
                });
            }
        }
        
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Error",
                "Fetching dates has failed.", "OK");
        }
        
        finally
        {
            clearWebsiteSelection();
        }
    }

    private void clearWebsiteSelection()
    {
        SelectedWebsite = null;
        OnPropertyChanged(nameof(SelectedWebsite));
    }
}