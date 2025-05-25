using AADConsoleTest.B2CConsole;
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
	class Program_X
	{
		// TODO: Replace these with your values
		private const string TenantId = "b7038365-c789-42c8-90f6-19f22aac9376"; 
		private const string ClientId = "2dbaa22e-63c2-4f5d-8936-8be140995851";
		private static readonly string[] Scopes = { }; //{ "User.Read" };


		// Should be hard coded - Should be stored securely. This is ok for development only.
		private const string ClientSecret = "dLb8Q~HzOVkj4x9bTSeSVPUl6t6iZ~SOhsgqEdtK";

		//static async Task Main(string[] args)
		//{ 
		//	await UserLogin.LoginWithUser();

		//	//await RequestToken();
		//	//await CreateUser();
		//}

		private static async Task RequestToken()
		{
			var app = PublicClientApplicationBuilder
				.Create(ClientId)
				.WithTenantId(TenantId)
				.WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
				.Build();

			Console.WriteLine("Requesting token via device code flow...");
			try
			{
				var result = await app.AcquireTokenWithDeviceCode(Scopes, deviceCodeResult =>
				{
					Console.WriteLine(deviceCodeResult.Message);
					return Task.CompletedTask;
				}).ExecuteAsync();

				Console.WriteLine("\n=== Authentication Successful ===");
				Console.WriteLine($"Access Token:\n{result.AccessToken.Substring(0, 80)}…");
				Console.WriteLine($"Expires On : {result.ExpiresOn}");
				Console.WriteLine($"Account     : {result.Account.Username}");
			}
			catch (MsalException ex)
			{
				Console.WriteLine($"\nERROR: {ex.ErrorCode}\n{ex.Message}");
			}
		}


		static async Task CreateUser_old()
		{
			// 1) Set up client-credentials auth
			var credential = new ClientSecretCredential(
				TenantId, ClientId, ClientSecret,
				new TokenCredentialOptions { AuthorityHost = AzureAuthorityHosts.AzurePublicCloud }
			);

			// 2) Build the GraphServiceClient
			var graphClient = new GraphServiceClient(credential);

			// 3) Define the new user
			var userEmail = "jane.doe@example.com";    // Pwd: Vudo657557
			var mailNick = userEmail.Split('@')[0];

			var newUser = new User
			{
				AccountEnabled = true,
				DisplayName = "Jane Doe",
				MailNickname = mailNick,
				UserPrincipalName = userEmail,
				PasswordProfile = new PasswordProfile
				{
					ForceChangePasswordNextSignIn = true,
					Password = "TempP@ssw0rd!"
				}
			};


			try
			{
				// 4) Create the user
				var createdUser = await graphClient.Users
					.PostAsync(newUser);

				Console.WriteLine($"✅ Created user: {createdUser.Id} ({createdUser.UserPrincipalName})");
			}
			catch (ServiceException ex)
			{
				Console.WriteLine($"❌ Graph API Error: {ex.Message}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Unexpected Error: {ex.Message}");
			}
		}

		// The email you want for sign-in
		private const string UserEmail = "jane.doe@example.com";
		//private const string UserName = "Jane Doe";
		private const string DisplayName = "Jane Doe";
		//private const string InitialPassword = "TempP@ssw0rd!"; // Strong temp password
		private const string TempPassword = "TempP@ssw0rd!";
		static async Task CreateUser()
		{
			// 1) Create an Azure.Identity credential
			var credential = new ClientSecretCredential(
				TenantId,
				ClientId,
				ClientSecret
			);

			// 2) Instantiate GraphServiceClient with that credential
			var graphClient = new GraphServiceClient(
				credential,
				new[] { "https://graph.microsoft.com/.default" }
			);

			// 3) Build the User object
			var newUser = new User
			{
				AccountEnabled = true,
				DisplayName = DisplayName,
				Identities = new List<ObjectIdentity>
				{
					new ObjectIdentity
					{
						SignInType       = "emailAddress",
						Issuer           = TenantId,
						IssuerAssignedId = UserEmail
					}
				},
				PasswordProfile = new PasswordProfile
				{
					ForceChangePasswordNextSignIn = false,
					Password = TempPassword
				},
				PasswordPolicies = "DisablePasswordExpiration"
			};

			try
			{
				// 4) Create the user via the new PostAsync pattern
				var created = await graphClient.Users
					.PostAsync(newUser);

				Console.WriteLine($"✔ Created user {created.DisplayName} ({created.Identities[0].IssuerAssignedId})");
			}
			catch (ODataError ex)
			{
				Console.Error.WriteLine($"❌ Graph error: {ex.Error.Code} – {ex.Error.Message}");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"❌ Unexpected error: {ex.Message}");
			}
		}
	}
}

