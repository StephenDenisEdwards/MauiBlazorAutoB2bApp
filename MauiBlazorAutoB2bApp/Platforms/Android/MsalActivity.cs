using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Microsoft.Identity.Client;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MauiBlazorAutoB2bApp.Platforms.Android
{

		[Activity(Exported = true)]
		[IntentFilter(new[] { Intent.ActionView },
			Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
			DataHost = "auth",
			DataScheme = "msal952374f2-2386-4541-a5ba-62023c022f18")]
		public class MsalActivity : BrowserTabActivity
		{
		}
}
