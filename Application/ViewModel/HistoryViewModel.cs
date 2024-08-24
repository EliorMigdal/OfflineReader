using System.Collections.ObjectModel;
using Application.Service.Remote;
using BusinessLogic.SupportedWebsite;

namespace Application.ViewModel;

public class HistoryViewModel : BaseViewModel
{
    public ObservableCollection<SupportedWebsite> SupportedWebsites { get; } = new();
    private readonly ServerAPI rm_ServerAPI = ServerAPI.Instance;

    public HistoryViewModel()
    {
        initializeSupportedWebsites();
    }

    private async void initializeSupportedWebsites()
    {
        List<SupportedWebsite> supportedWebsites = await rm_ServerAPI.LoadSupportedWebsites();

        foreach (SupportedWebsite supportedWebsite in supportedWebsites)
        {
            SupportedWebsites.Add(supportedWebsite);
        }
    }
}