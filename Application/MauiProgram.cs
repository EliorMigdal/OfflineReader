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
        
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        builder.Services.AddSingleton<SettingsViewModel>();
        builder.Services.AddSingleton<SettingsPage>();
        
        return builder.Build();
    }
}