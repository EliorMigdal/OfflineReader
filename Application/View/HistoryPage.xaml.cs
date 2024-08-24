using Application.ViewModel;

namespace Application.View;

public partial class HistoryPage
{
    public HistoryPage(HistoryViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}