using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;

namespace Metrics.Core.MVC
{
    public class MongoDataService
    {
        private readonly MongoService _mongoService;

        public MongoDataService(MongoService mongoService)
        {
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
            try
            {
                await _mongoService.CreateAsync(m);
                return new OkObjectResult("OK");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
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
            try
            {
                await _mongoService.CreateAsync(m);
                return new OkObjectResult("OK");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task Delete(int type, DateTime dt, string username)
        {
            var m = await _mongoService.GetAsync(type, username, 1000, 0);
            m = m.Where(x => x.Date == dt).ToList();
            foreach (var item in m)
            {
                await _mongoService.RemoveAsync(item.id);
            }
        }

        public async Task<List<Metric>> Get(int type, string username)
        {
            return await _mongoService.GetAsync(type, username, 1000, 0);
        }

        public async Task<List<Metric>> GetAll()
        {
            return await _mongoService.GetAsync();
        }
    }
}
