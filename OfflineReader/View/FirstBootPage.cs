using OfflineReader.ViewModel;
using OfflineReader.Model;

namespace OfflineReader.View;

public partial class FirstBootPage : ContentPage
{
    public FirstBootPage()
    {
        InitializeComponent();
        BindingContext = new FirstBootViewModel();
    }

    private void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        var switchControl = sender as Switch;
        var website = (SupportedWebsite)switchControl?.BindingContext;
        var viewModel = BindingContext as FirstBootViewModel;

        viewModel?.OnToggleSwitch(website);
    }
}