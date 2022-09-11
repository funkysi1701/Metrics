using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.Azure.Cosmos;
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
var databaseOld = cosmosClientOld.GetDatabase(config["DatabaseName"]);
var containerOld = databaseOld.GetContainer(config["ContainerName"]);
var opt = new MyMongoDatabaseSettings
{
    ConnectionString = config["ConnectionString"],
    DatabaseName = config["DatabaseName"],
    CollectionName = config["CollectionName"]
};
var iopt = Options.Create(opt);
var mongoService = new MongoService(iopt);
Console.WriteLine("Enter 0 - 22");
var type = Console.ReadLine();
if (type != null)
{
    await CheckKey(type);
}

async Task CheckKey(string? type)
{
    if (int.TryParse(type, out int Mtype) && Mtype >= 0 && Mtype <= 22)
    {
        var m = containerOld.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions { MaxItemCount = -1 }).Where(x => x.Type == Mtype).ToList();
        Console.WriteLine($"Type = {Mtype}");
        foreach (var item in m)
        {
            try
            {
                await mongoService.CreateAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine(item.Date?.ToString("yyyy-MM-dd HH:mm"));
        }
    }
    else
    {
        Environment.Exit(1);
    }

    Console.WriteLine("Enter 0 - 22");
    type = Console.ReadLine();
    if (type != null)
    {
        await CheckKey(type);
    }
}
