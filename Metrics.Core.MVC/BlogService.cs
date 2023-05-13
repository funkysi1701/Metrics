using Metrics.Core.Service;
using Metrics.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Metrics.Core.MVC
{
    public class BlogService
    {
        private readonly MongoDataService Chart;
        private IConfiguration Configuration { get; set; }

        public BlogService(IConfiguration Configuration, MongoService mongoService)
        {
            this.Configuration = Configuration;
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetBlogCount(ILogger log, string url, MetricType Type)
        {
            var count = DOXML(url, log);
            return await Chart.SaveData(count, Type, Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701");
        }

        private static int DOXML(string url, ILogger log)
        {
            var count = XDocument
                .Load(url)
                .XPathSelectElements("//item")
                .Count();
            log.LogInformation("{count} posts found", count);
            return count;
        }
    }
}
