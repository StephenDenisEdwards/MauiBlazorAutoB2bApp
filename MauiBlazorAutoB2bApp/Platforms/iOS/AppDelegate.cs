﻿using Foundation;
using Microsoft.Identity.Client;
using UIKit;

namespace MauiBlazorAutoB2bApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
    {
	    var urlString = url.AbsoluteString;

	    if (AuthenticationContinuationHelper.IsBrokerResponse(urlString))
	    {
		    // Broker flow (company portal, Intune, etc.)
		    AuthenticationContinuationHelper.SetBrokerContinuationEventArgs(url);
		    return true;
	    }

	    // Standard interactive web auth
	    return AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
    }
}
