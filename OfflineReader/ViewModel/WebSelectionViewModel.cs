namespace OfflineReader.ViewModel;

public partial class WebSelectionViewModel : BaseViewModel
{
    public ObservableCollection<SupportedWebsite> SupportedWebsites { get; set; } = new ObservableCollection<SupportedWebsite>
    {
        new SupportedWebsite(),
        new SupportedWebsite("mako"),
        new SupportedWebsite("CNN"),
        new SupportedWebsite("walla!"),
        new SupportedWebsite("BBC"),
        new SupportedWebsite("Reuters")
    };
}