using Microsoft.Extensions.Logging;
using OfflineReader.View;
using OfflineReader.ViewModel;

namespace OfflineReader;

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

        Routing.RegisterRoute(nameof(TestView), typeof(TestView));
        builder.Services.AddTransient<TestViewModel>();
        builder.Services.AddTransient<TestView>();

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        Routing.RegisterRoute(nameof(ReaderPage), typeof(ReaderPage));
        builder.Services.AddSingleton<ReaderPage>();

        Routing.RegisterRoute(nameof(WebSelectionPage), typeof(WebSelectionPage));
        builder.Services.AddSingleton<WebSelectionViewModel>();
        builder.Services.AddSingleton<WebSelectionPage>();

        Routing.RegisterRoute(nameof(SavedArticlesPage), typeof(SavedArticlesPage));
        builder.Services.AddSingleton<SavedArticlesViewModel>();
        builder.Services.AddSingleton<SavedArticlesPage>();

        return builder.Build();
    }
}