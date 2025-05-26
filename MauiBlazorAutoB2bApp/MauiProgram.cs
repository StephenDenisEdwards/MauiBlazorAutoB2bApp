//using MauiBlazorAutoB2bApp.MSALClient;

using MauiBlazorAutoB2bApp.MSALClient;
using Microsoft.Extensions.Logging;
using MauiBlazorAutoB2bApp.Shared.Services;
using MauiBlazorAutoB2bApp.Services;
using Microsoft.Maui.Devices;

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
		builder.Services.AddScoped<WeatherService>();
		//builder.Services.AddScoped(sp =>
		//	new WeatherService(new HttpClient
		//	{
		//		BaseAddress = new Uri("https://localhost:7038/")
		//	})
		//);

		// MauiProgram.cs
//		builder.Services.AddHttpClient("MyApi", client =>
//		{
//#if ANDROID
//			client.BaseAddress = new Uri("http://10.0.2.2:7038/");
//#elif IOS && !TARGET_IPHONE_SIMULATOR
//		        client.BaseAddress = new Uri("http://192.168.65.165:7038/");
//#else
//		        client.BaseAddress = new Uri("http://localhost:7038/");
//#endif
//		});


		//platform == DevicePlatform.Android

		builder.Services.AddHttpClient<WeatherService>(client =>
		{
			// same BaseAddress logic you already have…
			// client.BaseAddress = new Uri("http://localhost:7038/");
			//DevicePlatform pf = DeviceInfo.Current.Platform;
			//bool isSim = DeviceInfo.Current.DeviceType == DeviceType.Virtual;


#if ANDROID
			client.BaseAddress = new Uri("https://10.0.2.2:7038/");
			// client.BaseAddress = new Uri("http://10.0.2.2:5246/");
			
#elif IOS && !TARGET_IPHONE_SIMULATOR
		        client.BaseAddress = new Uri("https://192.168.65.165:7038/");
#else
		        client.BaseAddress = new Uri("https://localhost:7038/");
#endif

			//client.BaseAddress = new Uri(baseUrl);

			//string baseUrl = DeviceInfo.Platform switch
			//{
			//	DevicePlatform.Android => "http://10.0.2.2:7038/",
			//	DevicePlatform.iOS when !DeviceInfo.Idiom.Equals(DeviceIdiom.Simulator)
			//		=> "http://192.168.65.165:7038/",
			//	_ => "http://localhost:7038/"
			//};

			//client.BaseAddress = new Uri(baseUrl);


		}).ConfigurePrimaryHttpMessageHandler(() =>
			// WARNING: This handler accepts any SSL certificate. Use only for development purposes.

			new HttpClientHandler
			{
				// accept any cert (dev only!)
				ServerCertificateCustomValidationCallback =
					HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
			});



		//builder.Services.AddScoped(sp =>
		//	new WeatherService(new HttpClient
		//	{
		//		BaseAddress = new Uri("https://localhost:7038/")
		//	})
		//);


		builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

	    // Register MSALClientHelper with Entra External ID B2C config
	    builder.Services.AddSingleton<MSALClientHelper>(sp =>
	    {
		    var config = new AzureAdConfig
		    {
			    ClientId = "your-client-id",
			    Authority = "https://yourtenant.b2clogin.com/tfp/yourtenant/B2C_1_signupsignin",
			    TenantId = "your-tenant-id",
			    CacheFileName = "msalcache.dat",
			    CacheDir = FileSystem.AppDataDirectory
		    };
		    return new MSALClientHelper(config);
	    });


		return builder.Build();
    }
}
