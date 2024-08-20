using CommunityToolkit.Mvvm.ComponentModel;

namespace Application.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    public bool IsNotBusy => !IsBusy;
}