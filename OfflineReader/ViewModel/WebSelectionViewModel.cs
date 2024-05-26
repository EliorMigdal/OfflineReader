namespace OfflineReader.ViewModel;

public partial class WebSelectionViewModel : BaseViewModel
{
    public ObservableCollection<SupportedWebsite> SupportedWebsites { get; set; } = new ObservableCollection<SupportedWebsite>
    {
        new SupportedWebsite()
    };
}