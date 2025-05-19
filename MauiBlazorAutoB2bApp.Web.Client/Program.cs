using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MauiBlazorAutoB2bApp.Shared.Services;
using MauiBlazorAutoB2bApp.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the MauiBlazorAutoB2bApp.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
