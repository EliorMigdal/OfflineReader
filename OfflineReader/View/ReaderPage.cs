namespace OfflineReader.View;

public partial class ReaderPage : ContentPage
{
    public ReaderPage(ReaderPageViewModel i_ReaderViewModel)
    {
        InitializeComponent();
        BindingContext = i_ReaderViewModel;
    }
}
