using Application.ViewModel;

namespace Application.View;

public partial class ChooseSitesPage
{
    public ChooseSitesPage(ChooseSitesViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
    
}