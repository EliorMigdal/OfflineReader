using Application.ViewModel;

namespace Application.View;

public partial class AutoDownloadConfigPage
{
    public AutoDownloadConfigPage(ChooseSitesViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
    
}