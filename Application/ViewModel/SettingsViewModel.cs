using Application.View;
using System.Windows.Input;

namespace Application.ViewModel;

public class SettingsViewModel : BaseViewModel
{
    public ICommand ChooseSitesClickedCommand { get; private set; } = null!;
    public ICommand AutoDownloadClickedCommand { get; private set; } = null!;
    public ICommand NotificationsClickedCommand { get; private set; } = null!;

    public SettingsViewModel()
    {
        ChooseSitesClickedCommand = new Command(async () => await onChooseSitesCommand());
        AutoDownloadClickedCommand = new Command(async () => await onAutoDownloadCommand());
        NotificationsClickedCommand = new Command(async () => await onNotificationsCommand());
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

    //private async Task onAutoDownloadCommand()
    //{
    //    if (IsBusy)
    //        return;

    //    try
    //    {
    //        await Shell.Current.GoToAsync(nameof(ChooseSitesPage), true);

    //    }

    //    finally
    //    {
    //        IsBusy = false;
    //    }
    //}


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

    private async Task onNotificationsCommand()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        try
        {
            // Navigate to NotificationsPage or handle the command
        }
        finally
        {
            IsBusy = false;
        }
    }
}