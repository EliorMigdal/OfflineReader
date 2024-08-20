using Application.ViewModel;

namespace Application.View;

public partial class TrendingPage
{
    public TrendingPage(TrendingViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}