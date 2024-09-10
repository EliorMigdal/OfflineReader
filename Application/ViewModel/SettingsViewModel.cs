using Application.View;
using AsyncAwaitBestPractices.MVVM;
using System.Windows.Input;

namespace Application.ViewModel;

public class SettingsViewModel : BaseViewModel //, IQueryAttributable
{
    public ICommand ChooseSitesClickedCommand { get; private set; } = null!;
    public ICommand AutoDownloadClickedCommand { get; private set; } = null!;

    public SettingsViewModel()
    {
        ChooseSitesClickedCommand = new AsyncCommand(onChooseSitesCommand);
        AutoDownloadClickedCommand = new AsyncCommand(onAutoDownloadCommand);
    }

    private async Task onChooseSitesCommand()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            await Shell.Current.GoToAsync(nameof(ChooseSitesPage), true);
        }

        finally
        {
            IsBusy = false;
        }
    }

    private async Task onAutoDownloadCommand()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            await Shell.Current.GoToAsync(nameof(AutoDownloadConfigPage), true);
        }
        finally
        {
            IsBusy = false;
        }
    }
}