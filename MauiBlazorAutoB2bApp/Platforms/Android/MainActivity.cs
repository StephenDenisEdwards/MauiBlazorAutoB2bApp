using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Microsoft.Identity.Client;

namespace MauiBlazorAutoB2bApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
	ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
	                       ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	public static MainActivity Instance { get; private set; }

	protected override void OnCreate(Bundle savedInstanceState)
	{
		Log.Debug("MainActivity", "Debug message: Start OnCreate() called.");

		Instance = this;       // capture the activity

		//Console.WriteLine("MainActivity.OnCreate called");
		base.OnCreate(savedInstanceState);

		// configure platform specific params
		//PlatformConfig.Instance.RedirectUri = $"msal{PublicClientSingleton.Instance.MSALClientHelper.AzureAdConfig.ClientId}://auth";
		//PlatformConfig.Instance.ParentWindow = this;
		Log.Debug("MainActivity", "Debug message: End OnCreate() called.");
	}

	protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
	{
		Log.Debug("MainActivity", "Debug message: Start OnActivityResult() called.");
		base.OnActivityResult(requestCode, resultCode, data);
		AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
		Log.Debug("MainActivity", "Debug message: End OnActivityResult() called.");
	}
}


// /MyApp.Android/Services/ParentWindowProvider.cs