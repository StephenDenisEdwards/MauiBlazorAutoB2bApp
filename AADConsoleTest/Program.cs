using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;

//using Microsoft.Graph.Auth;

namespace AADConsoleTest
{
	public static class AuthConfig
	{
		public static string ClientId { get; } = "7eefa512-4258-475c-8204-c7b8ffde3039";
		public static string Authority { get; } = "https://tinglercustomers.ciamlogin.com/";
		public static IEnumerable<string> Scopes { get; } = new string[] { };
	}
	class Program
	{
		private static IPublicClientApplication _app;

		static async Task Main(string[] args)
		{
			_app = PublicClientApplicationBuilder.Create(AuthConfig.ClientId)
				.WithAuthority(AuthConfig.Authority)
				.WithDefaultRedirectUri() // loopback redirect uri is set here
				.Build();

			await AcquireToken();
		}

		private static async Task AcquireToken()
		{
			var accounts = await _app.GetAccountsAsync();
			AuthenticationResult result;

			if (accounts.Count() == 1)
			{
				try
				{
					result = await _app.AcquireTokenSilent(AuthConfig.Scopes, accounts.FirstOrDefault())
						.ExecuteAsync();
				}
				catch (MsalUiRequiredException)
				{
					result = await _app.AcquireTokenInteractive(AuthConfig.Scopes)
						.WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
						.ExecuteAsync();
				}
			}
			else if (accounts.Count() > 1)
			{
				Console.WriteLine("Multiple accounts found. Please select an account to use:");
				foreach (var account in accounts)
				{
					Console.WriteLine(account.Username);
				}
				return;
			}
			else
			{
				result = await _app.AcquireTokenInteractive(AuthConfig.Scopes)
					.WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
					.ExecuteAsync();
			}

			Console.WriteLine($"Access Token: {result.AccessToken}");
			Console.WriteLine($"Account: {result.Account.Username}");
			Console.WriteLine($"Expires On: {result.ExpiresOn}");

			Console.WriteLine("Press any key to exit...");
			Console.ReadLine();
		}
	}
}
