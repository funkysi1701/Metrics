using Metrics.Core.Service;
using Metrics.Model;
using Metrics.Model.Enum;
using Microsoft.Azure.Cosmos;

namespace Metrics.Console
{
    public static class KeyPress
    {
        public static async Task CheckKey(MetricType type, Container containerOld, MongoService mongoService)
        {
            var m = containerOld.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions { MaxItemCount = -1 }).Where(x => x.Type == type).OrderByDescending(x => x.Date).ToList();
            System.Console.WriteLine($"Type = {type}");
            foreach (var item in m)
            {
                try
                {
                    await mongoService.CreateAsync(item);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString().Substring(0, 200));
                }
                System.Console.WriteLine($"{item.Date?.ToString("yyyy-MM-dd HH:mm")} {item.Type}");
            }
            System.Console.WriteLine($"Type = {type}");
        }
    }
}
