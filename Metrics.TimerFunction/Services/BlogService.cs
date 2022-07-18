using Metrics.Core;
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

        public async Task GetBlogCount(ILogger log)
        {
            var url = Configuration.GetValue<string>("RSSFeed");

            var count = XDocument
                .Load(url)
                .XPathSelectElements("//item")
                .Count();
            log.LogInformation($"{count} posts found");
            await Chart.SaveData(count, (int)MetricType.Blog, Configuration.GetValue<string>("Username1"));
        }

        public async Task GetOldBlogCount(ILogger log)
        {
            var url = Configuration.GetValue<string>("OldRSSFeed");

            var count = XDocument
                .Load(url)
                .XPathSelectElements("//item")
                .Count();
            log.LogInformation($"{count} posts found");
            await Chart.SaveData(count, (int)MetricType.OldBlog, Configuration.GetValue<string>("Username1"));
        }
    }
}
