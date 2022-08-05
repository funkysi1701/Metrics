using Metrics.Core;
using Metrics.TimerFunction.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metrics.TimerFunction
{
    public class Chart
    {
        private readonly MongoService _mongoService;

        public Chart(MongoService mongoService)
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
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task Delete(int type, DateTime dt)
        {
            var m = await _mongoService.GetAsync();
            m = m.Where(x => x.Type == type && x.Date == dt).ToList();
            foreach (var item in m)
            {
                await _mongoService.RemoveAsync(item.id);
            }
        }

        public async Task<List<Metric>> Get(int type)
        {
            List<Metric> metrics = await _mongoService.GetAsync();
            return metrics.Where(x => x.Type == type).ToList();
        }

        public async Task<List<Metric>> GetAll()
        {
            return await _mongoService.GetAsync();
        }
    }
}
