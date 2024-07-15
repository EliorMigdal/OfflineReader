namespace OfflineReader.View;

public partial class FirstBootView : ContentPage
{
    public FirstBootView()
    {
        InitializeComponent();
        BindingContext = new FirstBootViewModel();
    }

    private void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        var switchControl = (Microsoft.Maui.Controls.Switch)sender;
        var website = (SupportedWebsite)switchControl.BindingContext;
        var viewModel = (FirstBootViewModel)BindingContext;

        viewModel.OnToggleSwitch(website);
    }
}