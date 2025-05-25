using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AADConsoleTest
{
	using System;
	using System.Net.Http;
	using System.Threading.Tasks;
	using Microsoft.Identity.Client;

	namespace B2CConsole
	{

		using System;
		using System.Threading.Tasks;
		using Microsoft.Identity.Client;

		namespace B2CConsole
		{
			class Program
			{
				private const string TenantShortName = "tinglermauib2c";
				private const string TenantFullName = "tinglermauib2c.onmicrosoft.com";
				private const string Policy = "B2C_1_SignUpSignIn";
				private const string ClientId = "2dbaa22e-63c2-4f5d-8936-8be140995851";

				// ⚠️ Note the trailing /v2.0
				private static readonly string Authority =
					$"https://{TenantShortName}.b2clogin.com/tfp/{TenantFullName}/{Policy}/v2.0/";

				private static readonly string[] Scopes =
				{
					$"https://{TenantShortName}.b2clogin.com/api/API.Access"
				};

				static async Task Main()
				{
					var app = PublicClientApplicationBuilder
						.Create(ClientId)
						.WithB2CAuthority(Authority)
						.Build();

					try
					{
						var result = await app
							.AcquireTokenWithDeviceCode(Scopes, dc =>
							{
								Console.WriteLine(dc.Message);
								return Task.CompletedTask;
							})
							.ExecuteAsync();

						Console.WriteLine("\n✅ Token acquired!\n");
					}
					catch (MsalServiceException msalEx)
					{
						// Print full error response from B2C
						Console.Error.WriteLine("MsalServiceException:");
						Console.Error.WriteLine($"  ErrorCode:    {msalEx.ErrorCode}");
						Console.Error.WriteLine($"  Message:      {msalEx.Message}");
						Console.Error.WriteLine($"  Raw Response: {msalEx}");
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine($"Unexpected exception: {ex}");
					}
				}
			}
		}

		public class UserLogin_1
		{
			// From your app registration
			private const string Tenant = "b7038365-c789-42c8-90f6-19f22aac9376.onmicrosoft.com";
			private const string ClientId = "2dbaa22e-63c2-4f5d-8936-8be140995851";
			private const string Policy = "B2C_1_SignUpSignIn"; // your sign-in policy

			private static readonly string Authority =
				$"https://{Tenant}/tfp/{Tenant}/{Policy}/v2.0";

			// API scope you exposed
			private static readonly string[] Scopes =
				{ $"https://{Tenant}/api/API.Access" };

			public static async Task LoginWithUser()
			{
				var app = PublicClientApplicationBuilder.Create(ClientId)
					.WithB2CAuthority(Authority)
					.Build();

				try
				{
					// 1) Acquire token via Device Code
					var result = await app.AcquireTokenWithDeviceCode(Scopes, dc =>
						{
							Console.WriteLine(dc.Message);
							return Task.CompletedTask;
						})
						.ExecuteAsync();

					Console.WriteLine("Access token acquired.");

					// 2) Call your API
					using var http = new HttpClient();
					http.DefaultRequestHeaders.Authorization =
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

					var resp = await http.GetAsync("https://api.myapp.com/data");
					Console.WriteLine(resp.IsSuccessStatusCode
						? await resp.Content.ReadAsStringAsync()
						: $"API error: {resp.StatusCode}");
				}
				catch (MsalException ex)
				{
					Console.Error.WriteLine($"Auth failed: {ex.ErrorCode} – {ex.Message}");
				}
			}
		}

		public class Program_2
		{
			// 1) Your B2C tenant "short name" (the part before .onmicrosoft.com)
			private const string TenantName = "tinglermauib2c";

			// 2) The B2C host you MUST use for authority and scopes
			private static readonly string TenantDomain = $"{TenantName}.b2clogin.com";

			// 3) Your user-flow (policy) name as configured in B2C
			private const string SignInPolicy = "B2C_1_SignUpSignIn";

			// 4) Your Console App’s client-ID (App registration → Overview)
			private const string ClientId = "2dbaa22e-63c2-4f5d-8936-8be140995851";

			// 5) Build the authority on b2clogin.com
			private static readonly string Authority =
				$"https://{TenantDomain}/tfp/{TenantName}.onmicrosoft.com/{SignInPolicy}/v2.0";

			// 6) The scope you exposed on your Web API registration
			private static readonly string[] Scopes =
			{
			//	$"https://{TenantDomain}/api/API.Access"
			};

			// public static async Task LoginWithUser()
			static async Task Main_X(string[] args)
			{
				var app = PublicClientApplicationBuilder.Create(ClientId)
					.WithB2CAuthority(Authority)
					.Build();

				try
				{
					var result = await app
						.AcquireTokenWithDeviceCode(Scopes, callback =>
						{
							// Printed to console for the user to complete auth
							Console.WriteLine(callback.Message);
							return Task.CompletedTask;
						}).ExecuteAsync().ConfigureAwait(false);

					Console.WriteLine("\n✅ Access token acquired!\n");

					// Example API call
					using var http = new HttpClient();
					http.DefaultRequestHeaders.Authorization =
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

					var resp = await http.GetAsync("https://api.myapp.com/data");
					if (resp.IsSuccessStatusCode)
					{
						Console.WriteLine(await resp.Content.ReadAsStringAsync());
					}
					else
					{
						Console.WriteLine($"API error: {resp.StatusCode}");
					}
				}
				catch (MsalException ex)
				{
					Console.Error.WriteLine($"❌ Authentication error: {ex.ErrorCode} — {ex.Message}");
				}
			}
		}
	}

}


