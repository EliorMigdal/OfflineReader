namespace OfflineReader.View;

public partial class WebSelectionPage : ContentPage
{
    public WebSelectionPage(WebSelectionViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}