# Authentication

I have a .NET MAUI Blazor Hybrid application. This application provides Android, IOS, Windows and Web interfaces. 
I want users to be able to signup and to authenticate using Microsft Entra External ID. They will be using their proviate emails as their user ID.
How would I approach this in the MAUI app.


Here’s a high-level approach to wire up Microsoft Entra External ID (the converged B2C CIAM) in your .NET MAUI Blazor Hybrid app so your users can sign-up/sign-in with their private emails:

---

## 1. Provision & configure your Entra External ID tenant

1. **Create an External ID (B2C) tenant** in the Entra admin center (or via the Visual Studio Code Entra Extension).
2. **Define a Sign-up / Sign-in user flow** (or custom policy) that collects email + password and any extra attributes you need.
3. (Optional) Add external IdPs (Google, Facebook, etc.) if you ever want social logins ([Microsoft Learn][1]).

---

## 2. Register your MAUI app in Azure

1. **App Registration → New registration**

   * **Name**: e.g. `Contoso.MauiB2C`.
   * **Supported account types**: “Accounts in this organizational directory only” (your External ID tenant).
2. **Authentication → Add a platform → Public client (mobile & desktop)**

   * **Redirect URI**:

     ```
     msal{ClientID}://auth
     ```

     (replace `{ClientID}` with your App’s Client ID) ([Microsoft Learn][2]).
3. **API Permissions** → Add Microsoft Graph delegated scopes: `openid`, `offline_access` → **Grant admin consent**.

---

## 3. Configure MAUI project for MSAL

### a) Install the MSAL.NET package

```shell
dotnet add package Microsoft.Identity.Client
```

### b) Create an MSAL “helper” singleton

In `/Services/MSALClientHelper.cs`:

```csharp
public static class MSALClientHelper
{
    // Initialize once at startup
    public static IPublicClientApplication PCA = PublicClientApplicationBuilder
        .Create("YOUR-CLIENT-ID")
        .WithB2CAuthority("https://<your-tenant>.b2clogin.com/tfp/<your-tenant>.onmicrosoft.com/B2C_1_SignUpSignIn")
        .WithRedirectUri($"msal{"YOUR-CLIENT-ID"}://auth")
        .Build();

    // Scopes your app needs (e.g. openid + any API scopes)
    public static string[] Scopes = { "openid", "offline_access" };
}
```

You’ll want your MSALClientHelper live in the **shared** code—i.e. inside your Shared project—because it’s pure .NET and will be consumed by all platforms (Android, iOS, Windows, Web).

A common pattern is:

1. **Create a `Services` folder** in your Shared project (if you don’t already have one).
2. **Add `MSALClientHelper.cs`** in that folder.
3. Make sure that any platform-specific bits (like the redirect URI scheme and the `WithParentActivityOrWindow(...)` hook) are passed in from your platform 项目 via abstraction or static configuration, but the MSAL client builder itself (the `.Create(...).WithB2CAuthority(...).Build()`) stays in shared code.

```text
/MySolution
  /MyApp.Android
  /MyApp.iOS
  /MyApp.Windows
  /MyApp.Web
  /MyApp.Shared
      /Services
          MSALClientHelper.cs    ← put it here
      /ViewModels
      /Views
      ...
```

That way any Blazor page or ViewModel in your Shared project can just call `MSALClientHelper.PCA` without worrying about which platform it’s on.




### c) Hook up platform callbacks

#### Android

* **Platforms/Android/AndroidManifest.xml**: ensure `intent-filter` with your scheme (`msal{ClientID}`) is registered.
* **Platforms/Android/MsalActivity.cs** (or MainActivity override):

  ```csharp
  protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
  {
      base.OnActivityResult(requestCode, resultCode, data);
      AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
  }
  ```

#### iOS

* **Platforms/iOS/AppDelegate.cs**:

  ```csharp
  public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
  {
      if (AuthenticationContinuationHelper.IsBrokerResponse(url))
      {
          AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
          return true;
      }
      return AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
  }
  ```
* Enable Keychain access in your `Entitlements.plist` so that silent renewals work ([Microsoft Learn][2]).

---

## 4. Wire up sign-in / sign-out in your Blazor UI

Because you’re in a Hybrid (Blazor WebView) app, you’ll invoke MSAL from C# and then drive your Razor UI:

```razor
@inject NavigationManager Nav
@code {
    async Task SignInAsync()
    {
        try
        {
            // Try silent first
            var result = await MSALClientHelper.PCA
                .AcquireTokenSilent(MSALClientHelper.Scopes, MSALClientHelper.PCA.GetAccounts().FirstOrDefault())
                .ExecuteAsync();

            // proceed if silent succeeded...
        }
        catch (MsalUiRequiredException)
        {
            // Fallback to interactive
            var result = await MSALClientHelper.PCA
                .AcquireTokenInteractive(MSALClientHelper.Scopes)
                .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                .ExecuteAsync();
        }

        // After you have a valid ID token / access token:
        Nav.NavigateTo("fetchdata"); 
    }

    async Task SignOutAsync()
    {
        var accounts = await MSALClientHelper.PCA.GetAccountsAsync();
        foreach (var acct in accounts)
            await MSALClientHelper.PCA.RemoveAsync(acct);
        Nav.NavigateTo("signout-confirm");
    }
}
```

---

## 5. Test on each platform

* **Web variant**: you can still host a Blazor WebAssembly version side-by-side; use the built-in `AddMsalAuthentication` Blazor extension for that ([Microsoft Learn][3]).
* **Mobile/Desktop**: run on emulator/device to verify the native browser flow and silent token cache.

---

### References & Samples

* Complete MAUI sample (CIAM) by Microsoft: ms-identity-ciam-dotnet-tutorial (Sign-in MAUI) ([Microsoft Learn][2])
* “A .NET MAUI app using MSAL.NET to authenticate users with Azure AD for Customers” (CIAM sample) ([Microsoft Learn][1])
* ASP.NET Core Blazor Hybrid auth guidance ([Microsoft Learn][4])

This setup gives you a fully branded, secure sign-up/sign-in experience in your Blazor Hybrid UI across Android, iOS, Windows and Web, backed by Entra External ID’s B2C CIAM service.

[1]: https://learn.microsoft.com/en-us/samples/azure-samples/ms-identity-ciam-dotnet-tutorial/ms-identity-ciam-dotnet-tutorial-2-sign-in-maui/?utm_source=chatgpt.com "A .NET MAUI app using MSAL.NET to authenticate users with Azure ..."
[2]: https://learn.microsoft.com/en-us/samples/azure-samples/ms-identity-ciam-dotnet-tutorial/ms-identity-ciam-dotnet-tutorial-2-sign-in-maui/ "A .NET MAUI app using MSAL.NET to authenticate users with Azure AD for Customers - Code Samples | Microsoft Learn"
[3]: https://learn.microsoft.com/en-us/aspnet/core/blazor/security/webassembly/hosted-with-azure-active-directory-b2c?view=aspnetcore-7.0&utm_source=chatgpt.com "Secure a hosted ASP.NET Core Blazor WebAssembly app with ..."
[4]: https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/security/?view=aspnetcore-9.0&utm_source=chatgpt.com "ASP.NET Core Blazor Hybrid authentication and authorization"



# Additional Information


## Hook up platform callbacks - you show Android and IoS but what about Windows and the Web? 

For the Windows (WinUI/Desktop) and the Web (Blazor WebAssembly) flavors of your Hybrid app, the “hook” looks a bit different:

---

### Windows (WinUI 3/.NET MAUI on Windows)

1. **Declare your URI scheme** in your `Package.appxmanifest` under the `<Extensions>` section. It should match the same custom scheme you registered (e.g. `msal{CLIENT_ID}`).

   ```xml
   <Extensions>
     <uap:Extension Category="windows.protocol">
       <uap:Protocol Name="msal{YOUR-CLIENT-ID}" />
     </uap:Extension>
   </Extensions>
   ```

2. **Override `OnActivated`** in your WinUI `App.xaml.cs` so MSAL can pick up the redirect response:

   ```csharp
   protected override void OnActivated(WindowActivatedEventArgs args)
   {
       base.OnActivated(args);

       // This hands off any Windows protocol activation (msal{…}://...) back to MSAL
       AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(args);
   }
   ```

3. **Ensure your MAUI Windows project has the same redirect URI** in Azure (it’ll look like `ms-app://{YourPackageFamilyName}/msal{YourClientId}` if you’re using the broker) and that it matches the scheme in code and manifest .

---

### Blazor WebAssembly (Web)

On the pure-Web side you don’t wire platform callbacks yourself; MSAL.js handles redirects in the browser. Instead you configure it in `Program.cs`:

```csharp
builder.Services.AddMsalAuthentication(options =>
{
    options.ProviderOptions.Authentication.Authority = 
        "https://<your-tenant>.b2clogin.com/tfp/<your-tenant>.onmicrosoft.com/B2C_1_SignUpSignIn";
    options.ProviderOptions.Authentication.ClientId = "<YOUR-CLIENT-ID>";
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
    // If you need additional scopes, add them here.
});
```

That extension will automatically register the redirect URIs you set up in Azure (e.g. `https://localhost:5001/authentication/login-callback`) and handle the browser’s `window.location` changes for you .

---

#### Summary

* **Windows**: Declare the `msal{ClientId}` protocol in your app manifest, override `OnActivated` in `App.xaml.cs` to call `AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(...)`.
* **Web**: Use `AddMsalAuthentication` in your Blazor WebAssembly host—no manual callback wiring needed.
