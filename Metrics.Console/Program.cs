﻿using Metrics.Core.Model;
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
            await CheckKey(i.ToString());
        }
    }
    else await CheckKey(type);
}
System.Diagnostics.Process.Start(Environment.ProcessPath);
Environment.Exit(0);

async Task CheckKey(string? type)
{
    if (int.TryParse(type, out int Mtype) && Mtype >= 0 && Mtype <= 22)
    {
        var m = containerOld.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions { MaxItemCount = -1 }).Where(x => x.Type == Mtype).OrderByDescending(x => x.Date).ToList();
        Console.WriteLine($"Type = {Mtype}");
        foreach (var item in m)
        {
            try
            {
                await mongoService.CreateAsync(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString().Substring(0, 200));
            }
            Console.WriteLine($"{item.Date?.ToString("yyyy-MM-dd HH:mm")} {item.Type}");
        }
        Console.WriteLine($"Type = {Mtype}");
    }
    else
    {
        Environment.Exit(1);
    }
}
