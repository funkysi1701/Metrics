using ImpSoft.OctopusEnergy.Api;
using Metrics.Core.MVC;
using Metrics.Core.Service;
using Metrics.Model;
using Metrics.TimerFunction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Metrics.TimerFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            builder.Services.AddScoped<GithubService>();
            builder.Services.AddScoped<DevToService>();
            builder.Services.AddScoped<PowerService>();
            builder.Services.AddScoped<BlogService>();
            builder.Services.AddScoped<MastodonService>();
            builder.Services.Configure<MyMongoDatabaseSettings>(config);
            builder.Services.AddSingleton<MongoService>();
            builder.Services.AddHttpClient<IOctopusEnergyClient, OctopusEnergyClient>()
                .ConfigurePrimaryHttpMessageHandler(h => new HttpClientHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.All
                });
        }
    }
}
