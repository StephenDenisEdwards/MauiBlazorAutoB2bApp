using Microsoft.Extensions.Logging;
using MauiBlazorAutoB2bApp.Shared.Services;
using MauiBlazorAutoB2bApp.Services;

namespace MauiBlazorAutoB2bApp;

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
            });

        //builder.Services.AddHttpClient<WeatherService>(client =>
        //{
	       // // client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
	       // // client.BaseAddress = new Uri("https://localhost:7250/"); // Use your API's URL
	       // client.BaseAddress = new Uri("https://localhost:7238/"); // Use your API's URL
        //});

		// Add device-specific services used by the MauiBlazorAutoB2bApp.Shared project
		builder.Services.AddSingleton<IFormFactor, FormFactor>();
		// Register WeatherService for dependency injection
//		builder.Services.AddScoped<WeatherService>();
		builder.Services.AddScoped(sp =>
			new WeatherService(new HttpClient
			{
				BaseAddress = new Uri("https://localhost:7038/")
			})
		);

		builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
