using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.Azure.Cosmos;

namespace Metrics.Console
{
    public static class KeyPress
    {
        public static async Task CheckKey(string? type, Container containerOld, MongoService mongoService)
        {
            if (int.TryParse(type, out int Mtype) && Mtype >= 0 && Mtype <= 22)
            {
                var m = containerOld.GetItemLinqQueryable<Metric>(true, null, new QueryRequestOptions { MaxItemCount = -1 }).Where(x => x.Type == Mtype).OrderByDescending(x => x.Date).ToList();
                System.Console.WriteLine($"Type = {Mtype}");
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
                System.Console.WriteLine($"Type = {Mtype}");
            }
            else
            {
                Environment.Exit(1);
            }
        }
    }
}
