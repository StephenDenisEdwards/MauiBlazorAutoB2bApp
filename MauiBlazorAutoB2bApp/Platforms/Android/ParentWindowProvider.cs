using MauiBlazorAutoB2bApp.Shared.Services;

namespace MauiBlazorAutoB2bApp.Android;
public class ParentWindowProvider : IParentWindowProvider
{
	public object GetParentWindowOrActivity()
		=> MainActivity.Instance;    // the static you set in MainActivity.OnCreate
}