namespace OfflineReader.View;

public partial class SavedArticlesPage : ContentPage
{
    public SavedArticlesPage(SavedArticlesViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}