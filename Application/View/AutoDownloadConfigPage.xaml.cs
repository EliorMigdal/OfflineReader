using Application.ViewModel;

namespace Application.View;

public partial class AutoDownloadConfigPage
{
    public AutoDownloadConfigPage(AutoDownloadConfigViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
    
}