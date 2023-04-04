using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Metrics.StaticFunction
{
    public class Metric
    {
        private readonly IConfiguration Configuration;

        public Metric(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [FunctionName("Metrics")]
        public async Task<IActionResult> Metrics(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("testing");
        }
    }
}
