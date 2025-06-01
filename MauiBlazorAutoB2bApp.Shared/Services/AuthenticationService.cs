using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace MauiBlazorAutoB2bApp.Shared.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IPublicClientApplication _pca;
		private readonly string[] _scopes;
		private readonly IParentWindowProvider _parent;

		public AuthenticationService(
			IPublicClientApplication pca,
			IConfiguration config,
			IParentWindowProvider parent)
		{
			_pca = pca;
			_parent = parent;
			//			_scopes = config.GetSection("AzureAd:Scopes").Get<string[]>(); // Ensure 'Microsoft.Extensions.DependencyInjection' is referenced  

			_scopes = config.GetSection("AzureAd:Scopes")
				.GetChildren()
				.Select(section => section.Value)
				.ToArray();  // Ensure 'Microsoft.Extensions.DependencyInjection' is referenced  
		}

		public async Task<AuthenticationResult> SignInAsync()
		{
			try
			{
				//var accounts = await _pca.GetAccountsAsync();
				//return await _pca.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
				//	.ExecuteAsync();

				throw new MsalUiRequiredException("Test", "Test exception for testing purposes");

			}
			catch (MsalUiRequiredException me)
			{
				var parent = _parent.GetParentWindowOrActivity();

				AcquireTokenInteractiveParameterBuilder? aquireFunction = _pca
					.AcquireTokenInteractive(_scopes)

#if DEBUG
					// .WithUseEmbeddedWebView(true)
					.WithUseEmbeddedWebView(false)
#endif

					.WithParentActivityOrWindow(parent);

				//var result = MainThread.InvokeOnMainThreadAsync(async () =>
				//{
				//	result = await aquireFunction
				//		.ExecuteAsync();

				//});

				AuthenticationResult result = null!;
				// Ensure interactive call runs on UI thread
				//await MainThread.InvokeOnMainThreadAsync(async () =>
				//{
					result = await aquireFunction
						.ExecuteAsync()
						.ConfigureAwait(false);
				//});

				return result;




				//AuthenticationResult? result = await aquireFunction
				//	.ExecuteAsync();

				//return result;

				/*

				Calling from a non-UI thread ???
				Always invoke AcquireTokenInteractive on the main thread. You can wrap it in:

				MainThread.InvokeOnMainThreadAsync(async () =>
				   {
				       await _pca.AcquireTokenInteractive(_scopes)
				                 .WithParentActivityOrWindow(...)
				                 .ExecuteAsync();
				   });

				*/

			}
		}

		public async Task SignOutAsync()
		{
			var accounts = await _pca.GetAccountsAsync();
			foreach (var acct in accounts)
				await _pca.RemoveAsync(acct);
		}
	}
}
