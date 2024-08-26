using System.Collections.ObjectModel;
using System.Windows.Input;
using Application.Service.Remote;
using Application.View;
using AsyncAwaitBestPractices.MVVM;
using BusinessLogic.Article.Partials;

namespace Application.ViewModel;

public class DatePickerViewModel : BaseViewModel, IQueryAttributable
{
    public ObservableCollection<string> AvailableDates { get; set; } = new();
    public string? SelectedDate { get; set; }
    private string? SelectedWebsite { get; set; }
    public ICommand DateSelectedCommand { get; set; }
    private List<string>? Dates { get; set; }
    private readonly ServerAPI rm_ServerAPI = ServerAPI.Instance;

    public DatePickerViewModel()
    {
        DateSelectedCommand = new AsyncCommand(loadArticlesFromDate);
    }

    private async Task loadArticlesFromDate()
    {
        try
        {
            if (SelectedWebsite is not null && SelectedDate is not null)
            {
                List<OuterArticle> outerArticles = await rm_ServerAPI.GetDatedArticles(SelectedWebsite, SelectedDate);

                await Shell.Current.GoToAsync(nameof(HistoryList), true, new Dictionary<string, object>
                {
                    { "Date", SelectedDate }, { "Website", SelectedWebsite }, { "Articles", outerArticles }
                });
            }
        }

        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Error",
                "Fetching articles has failed.", "OK");
        }
        
        finally
        {
            clearDateSelection();
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (Dates != null && Dates.Count != 0) return;
        Dates = query["Dates"] as List<string>;
        SelectedWebsite = query["Website"] as string;
        Dates?.ForEach(date => AvailableDates.Add(date));
    }

    private void clearDateSelection()
    {
        SelectedDate = null;
        OnPropertyChanged(nameof(SelectedDate));
    }
}