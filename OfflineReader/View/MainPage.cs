namespace OfflineReader.View;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;

        if (IsFirstBoot())
        {
            Debug.WriteLine($"Identified as first boot!");
            DisplayFirstTimePage();
        }

        else
        {
            _ = i_ViewModel.GetArticlesAsync();
        }        
    }

    private bool IsFirstBoot()
    {
        return !ConfigService.DoesConfigFileExist();
    }

    private async void DisplayFirstTimePage()
    {
        try
        {
            await Shell.Current.Navigation.PushModalAsync(new FirstBootView());
        }

        finally
        {
            _ = (BindingContext as MainViewModel).GetArticlesAsync();
        }
    }
}