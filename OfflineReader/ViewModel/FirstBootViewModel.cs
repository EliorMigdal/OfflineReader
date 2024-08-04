using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using OfflineReader.Model;
using OfflineReader.Service.Local;

namespace OfflineReader.ViewModel;

public class FirstBootViewModel : BaseViewModel
{
    public ICommand ContinueCommand { get; }
    public ICommand ToggleSwitchCommand { get; }

    public ObservableCollection<SupportedWebsite> SupportedWebsites { get; set; } = new ObservableCollection<SupportedWebsite>
    {
        new SupportedWebsite("ynet", "https://www.ynet.co.il"),
        new SupportedWebsite("mako", "https://www.mako.co.il"),
        new SupportedWebsite("CNN", "https://cnn.com"),
        new SupportedWebsite("walla!", "https://www.walla.co.il"),
        new SupportedWebsite("BBC", "https://www.bbc.com"),
        new SupportedWebsite("Reuters", "https://www.reuters.com")
    };

    public FirstBootViewModel()
    {
        ContinueCommand = new Command(OnContinue);
        ToggleSwitchCommand = new Command<SupportedWebsite>(OnToggleSwitch);
    }

    private async void OnContinue()
    {
        List<SupportedWebsite> selectedWebsites = GetSelectedWebsites();
        ConfigService.AddWebsitesToConfigFile(selectedWebsites);
        Debug.WriteLine($"Created config file apparentley");
        await Shell.Current.Navigation.PopModalAsync();
    }

    public List<SupportedWebsite> GetSelectedWebsites()
    {
        return SupportedWebsites.Where(website => website.Selection).ToList();
    }

    public void OnToggleSwitch(SupportedWebsite website)
    {
        if (website != null)
        {
            website.Selection = !website.Selection;
            Debug.WriteLine($"Toggled: {website.Name}");
        }
    }
}