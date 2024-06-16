namespace OfflineReader.View;

public partial class TestView : ContentPage
{
    public TestView(TestViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}