using Microsoft.Extensions.Logging;

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

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        Routing.RegisterRoute(nameof(ReaderPage), typeof(ReaderPage));
        builder.Services.AddSingleton<ReaderPage>();

        Routing.RegisterRoute(nameof(WebSelectionPage), typeof(WebSelectionPage));
        builder.Services.AddSingleton<WebSelectionViewModel>();
        builder.Services.AddSingleton<WebSelectionPage>();

        return builder.Build();
    }
}