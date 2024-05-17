using System.Windows.Input;

namespace OfflineReader.ViewModel;

public class MenuViewModel : BindableObject
{
    private ShellItem _selectedItem;
    public string SelectedItemRoute { get; set; }
    public ObservableCollection<ShellItem> MenuItems { get; }
    public ShellItem SelectedItem
    {
        get => _selectedItem;

        set
        {
            if (_selectedItem != value)
            {
                _selectedItem = value;
                OnPropertyChanged();
                OnSelectedItemChanged();
            }
        }
    }

    public ICommand MenuItemSelectedCommand { get; }

    public MenuViewModel()
    {
        MenuItems = new ObservableCollection<ShellItem>
            {
                new FlyoutItem { Title = "Main Page", Icon = "icon_mainpage.png", Route = "MainPage" },
                new FlyoutItem { Title = "Settings", Icon = "icon_settings.png", Route = "SettingsPage" },
                new FlyoutItem { Title = "About", Icon = "icon_about.png", Route = "AboutPage" }
            };

        MenuItemSelectedCommand = new Command(OnSelectedItemChanged);
    }

    private void OnSelectedItemChanged()
    {
        if (SelectedItem != null)
        {
            Shell.Current.GoToAsync($"//{SelectedItem.Route}");
        }
    }
}