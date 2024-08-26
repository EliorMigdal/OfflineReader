using Application.ViewModel;

namespace Application.View;

public partial class HistoryList
{
    public HistoryList(HistoryListViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}