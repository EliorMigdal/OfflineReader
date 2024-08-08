using OfflineReader.Model;
using OfflineReader.ViewModel;

namespace OfflineReader.View;

public partial class TestView : ContentPage
{
    public TestView()
    {
        InitializeComponent();
    }

    private void Slider_OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        double scaleFactor = e.NewValue - e.OldValue;
    
        foreach (var child in Content.Children)
        {
            if (child is not StackLayout layout) continue;
            
            foreach (var stackChild in layout.Children)
            {
                if (stackChild is Label label)
                {
                    label.FontSize += scaleFactor;
                }
            }
        }
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        if (BindingContext is not TestViewModel viewModel) return;
        viewModel.SaveArticle(SharedData.ParsedArticle);
    }
}