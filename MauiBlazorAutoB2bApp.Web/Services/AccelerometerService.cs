using MauiBlazorAutoB2bApp.Shared.Services;
using Microsoft.Maui.Devices.Sensors;

namespace MauiBlazorAutoB2bApp.Web.Services;

public class AccelerometerService : IAccelerometerService
{
	public ValueTask DisposeAsync()
	{
		throw new NotImplementedException();
	}

	public event EventHandler<AccelerometerChangedEventArgs>? OnReadingChanged;
	public void Start()
	{
		throw new NotImplementedException();
	}

	public void Stop()
	{
		throw new NotImplementedException();
	}
}