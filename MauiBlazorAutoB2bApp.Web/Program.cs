using MauiBlazorAutoB2bApp.Web.Components;
using MauiBlazorAutoB2bApp.Shared.Services;
using MauiBlazorAutoB2bApp.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

//builder.Services.AddHttpClient<WeatherService>();
//builder.Services.AddHttpClient<WeatherService>(client =>
//{
	// client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
	// client.BaseAddress = new Uri("https://localhost:7250/"); // Use your API's URL
	//client.BaseAddress = new Uri("https://localhost:7238/"); // Use your API's URL
//});
// Add device-specific services used by the MauiBlazorAutoB2bApp.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
//builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped(sp =>
	new WeatherService(new HttpClient
	{
		BaseAddress = new Uri("https://localhost:7038/")
	})
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(MauiBlazorAutoB2bApp.Shared._Imports).Assembly,
        typeof(MauiBlazorAutoB2bApp.Web.Client._Imports).Assembly);

app.Run();
