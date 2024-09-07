using Application.View;
using Application.ViewModel;
using Microsoft.Extensions.Logging;

namespace Application;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        #if DEBUG
        builder.Logging.AddDebug();
        #endif
        
        builder.Services.AddSingleton(Connectivity.Current);

        Routing.RegisterRoute(nameof(ReadingPage), typeof(ReadingPage));
        builder.Services.AddTransient<ReaderViewModel>();
        builder.Services.AddTransient<ReadingPage>();

        Routing.RegisterRoute(nameof(TrendingPage), typeof(TrendingPage));
        builder.Services.AddSingleton<TrendingViewModel>();
        builder.Services.AddSingleton<TrendingPage>();
        
        Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
        builder.Services.AddSingleton<HistoryViewModel>();
        builder.Services.AddSingleton<HistoryPage>();
        
        Routing.RegisterRoute(nameof(DatePickerPage), typeof(DatePickerPage));
        builder.Services.AddTransient<DatePickerViewModel>();
        builder.Services.AddTransient<DatePickerPage>();
        
        Routing.RegisterRoute(nameof(HistoryList), typeof(HistoryList));
        builder.Services.AddTransient<HistoryListViewModel>();
        builder.Services.AddTransient<HistoryList>();
        
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();

        Routing.RegisterRoute(nameof(ChooseSitesPage), typeof(ChooseSitesPage));
        builder.Services.AddSingleton<ChooseSitesViewModel>();
        builder.Services.AddSingleton<ChooseSitesPage>();

        Routing.RegisterRoute(nameof(AutoDownloadConfigPage), typeof(AutoDownloadConfigPage));
        builder.Services.AddSingleton<AutoDownloadConfigViewModel>();
        builder.Services.AddSingleton<AutoDownloadConfigPage>();

        return builder.Build();
    }
}