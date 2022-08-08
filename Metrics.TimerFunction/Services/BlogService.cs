using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Metrics.TimerFunction.Services
{
    public class BlogService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }

        public BlogService(IConfiguration Configuration, MongoService mongoService)
        {
            this.Configuration = Configuration;
            Chart = new Chart(mongoService);
        }

        public async Task<IActionResult> GetBlogCount(ILogger log, string url, int Type)
        {
            var count = DOXML(url, log);
            return await Chart.SaveData(count, Type, Configuration.GetValue<string>("Username1"));
        }

        private static int DOXML(string url, ILogger log)
        {
            var count = XDocument
                .Load(url)
                .XPathSelectElements("//item")
                .Count();
            log.LogInformation($"{count} posts found");
            return count;
        }
    }
}
