using Metrics.Console;
using Metrics.Core.Service;
using Metrics.Model;
using Metrics.Model.Enum;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

Console.WriteLine("Hello, World!");
IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

CosmosClientBuilder cosmosClientBuilderOld = new(config["CosmosDBStringOld"]);
var cosmosClientOld = cosmosClientBuilderOld.Build();
var databaseOld = cosmosClientOld.GetDatabase(config["DatabaseNameOld"]);
var containerOld = databaseOld.GetContainer(config["ContainerName"]);
var opt = new MyMongoDatabaseSettings
{
    ConnectionString = config["ConnectionString"],
    DatabaseName = config["DatabaseName"],
    CollectionName = config["CollectionName"]
};
var iopt = Options.Create(opt);
var mongoService = new MongoService(iopt);
Console.WriteLine("Enter 0 - 22, or A for All, Q for Quit");
var type = Console.ReadLine();

if (type != null)
{
    if (type == "Q")
    {
        Environment.Exit(0);
    }
    else if (type == "A")
    {
        for (int i = 0; i < 23; i++)
        {
            await KeyPress.CheckKey((MetricType)i, containerOld, mongoService);
        }
    }
    else
    {
        if(int.TryParse(type, out int i))
        {
            await KeyPress.CheckKey((MetricType)i, containerOld, mongoService);
        }
    }
}
if (Environment.ProcessPath != null)
    System.Diagnostics.Process.Start(Environment.ProcessPath);
Environment.Exit(0);
