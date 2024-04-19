namespace OfflineReader.View;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel i_ViewModel)
	{
		InitializeComponent();
		BindingContext = i_ViewModel;
        _ = i_ViewModel.GetArticlesAsync();
	}
}