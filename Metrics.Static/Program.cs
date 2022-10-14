using BlazorApplicationInsights;
using Metrics.Static;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<ChartService>();
builder.Services.AddBlazorApplicationInsights(async applicationInsights =>
{
    await applicationInsights.SetInstrumentationKey(builder.Configuration.GetValue<string>("InstrumentationKey"));

    await applicationInsights.TrackPageView();
});
await builder.Build().RunAsync();
