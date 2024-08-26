using Application.ViewModel;

namespace Application.View;

public partial class DatePickerPage
{
    public DatePickerPage(DatePickerViewModel i_ViewModel)
    {
        InitializeComponent();
        BindingContext = i_ViewModel;
    }
}