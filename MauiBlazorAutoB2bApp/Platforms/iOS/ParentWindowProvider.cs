using MauiBlazorAutoB2bApp.Shared.Services;
using Foundation;
using UIKit;

namespace MauiBlazorAutoB2bApp.IoS;

public class ParentWindowProvider : IParentWindowProvider
{
	public object GetParentWindowOrActivity()
		=> UIApplication.SharedApplication.KeyWindow; 
}