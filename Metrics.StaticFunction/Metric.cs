using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Metrics.StaticFunction
{
    public class ProMetric
    {
        private readonly IConfiguration Configuration;

        public ProMetric(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [FunctionName("ProMetrics")]
        public async Task<IActionResult> ProMetrics(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("testing");
        }
    }
}
