using Android.App;
using Android.Content.PM;
using Android.OS;

namespace MauiBlazorAutoB2bApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
	ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
	                       ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	public static MainActivity Instance { get; private set; }

	protected override void OnCreate(Bundle savedInstanceState)
	{
		Instance = this;       // capture the activity

		//Console.WriteLine("MainActivity.OnCreate called");
		base.OnCreate(savedInstanceState);
	}
}

// /MyApp.Android/Services/ParentWindowProvider.cs