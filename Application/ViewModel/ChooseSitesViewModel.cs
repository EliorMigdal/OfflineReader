using Application.View;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Application.ViewModel;

public class ChooseSitesViewModel : BaseViewModel
{
    public class Site
    {
        public string Name { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }

    public ObservableCollection<Site> Sites { get; private set; }

    public ICommand SaveSitesCommand { get; private set; }

    public ChooseSitesViewModel()
    {
        Sites = new ObservableCollection<Site>();
        setNamesOfSites();
        SaveSitesCommand = new Command(OnSaveSites);
    }

    private void setNamesOfSites()
    {
        Sites.Add(new Site { Name = "Mako", IsSelected = false });
        Sites.Add(new Site { Name = "Ynet", IsSelected = false });
        Sites.Add(new Site { Name = "TheMarker", IsSelected = false });
        Sites.Add(new Site { Name = "Walla", IsSelected = false });
        Sites.Add(new Site { Name = "CNN", IsSelected = false });
        Sites.Add(new Site { Name = "BBC", IsSelected = false });
    }

    private void OnSaveSites()
    {
        var selectedSites = Sites.Where(site => site.IsSelected).ToList();

        // Logic to handle selected sites
        foreach (var site in selectedSites)
        {
        }
    }
}
