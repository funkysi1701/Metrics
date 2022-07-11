using Metrics.Core;
using Metrics.TimerFunction.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metrics.TimerFunction
{
    public class Chart
    {
        private readonly Container _container;
        private readonly MongoService _mongoService;

        public Chart(CosmosClient cosmosClient, IConfiguration Configuration, MongoService mongoService)
        {
            var _cosmosClient = cosmosClient;
            var _database = _cosmosClient.GetDatabase(Configuration.GetValue<string>("DatabaseName"));
            _container = _database.GetContainer(Configuration.GetValue<string>("ContainerName"));
            _mongoService = mongoService;
        }

        public async Task<IActionResult> SaveData(decimal value, int type, string username)
        {
            var m = new Metric
            {
                MetricId = DateTime.UtcNow.Ticks,
                id = DateTime.UtcNow.Ticks.ToString(),
                Date = DateTime.UtcNow,
                Type = type,
                Value = value,
                PartitionKey = "1",
                Username = username
            };
            await _mongoService.CreateAsync(m);
            return new OkResult();
        }

        public async Task<IActionResult> SaveDataCosmos(decimal value, int type, string username)
        {
            var m = new Metric
            {
                MetricId = DateTime.UtcNow.Ticks,
                id = DateTime.UtcNow.Ticks.ToString(),
                Date = DateTime.UtcNow,
                Type = type,
                Value = value,
                PartitionKey = "1",
                Username = username
            };
            await _container.CreateItemAsync(m);
            return new OkResult();
        }

        public async Task<IActionResult> SaveData(decimal value, int type, DateTime To, string username)
        {
            var m = new Metric
            {
                MetricId = DateTime.UtcNow.Ticks,
                id = DateTime.UtcNow.Ticks.ToString(),
                Date = To,
                Type = type,
                Value = value,
                PartitionKey = "1",
                Username = username
            };
            await _container.CreateItemAsync(m);
            return new OkResult();
        }

        public async Task Delete(int type, DateTime dt)
        {
            var m = _container.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions
            {
                MaxItemCount = -1,
            }).Where(x => x.Type == type && x.Date == dt).ToList();
            foreach (var item in m)
            {
                await _container.DeleteItemAsync<Metric>(item.id, new PartitionKey(item.PartitionKey));
            }
        }

        public List<Metric> Get(int type)
        {
            return _container.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions
            {
                MaxItemCount = -1,
            }).Where(x => x.Type == type).ToList();
        }

        public List<Metric> GetAll()
        {
            return _container.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions { MaxItemCount = -1 }).ToList();
        }
    }
}
