using Metrics.Core.MVC;
using Metrics.Core.Service;
using Metrics.IIS.Services;
using Metrics.Model.Enum;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;

namespace Metrics.IIS.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TwitterController : ControllerBase
    {
        private readonly TwitterService twitterService;
        private readonly TelemetryClient telemetry;
        private readonly MongoDataService Chart;
        private readonly string username = "funkysi1701";

        public TwitterController(TwitterService twitterService, TelemetryClient telemetry, MongoService mongoService)
        {
            this.twitterService = twitterService;
            this.telemetry = telemetry;
            Chart = new MongoDataService(mongoService);
        }

        /// <summary>
        /// SaveTwitterFollowers
        /// </summary>
        /// <remarks>SaveTwitterFollowers</remarks>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowers()
        {
            try
            {
                var result = await twitterService.GetTwitterFollowers(telemetry, username);
                var ob = result as OkObjectResult;
                var value = (decimal)ob.Value;
                if (value > 0)
                {
                    return await Chart.SaveData(value, MetricType.TwitterFollowers, username);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                throw;
            }
        }

        /// <summary>
        /// SaveTwitterFollowing
        /// </summary>
        /// <remarks>SaveTwitterFollowing</remarks>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowing()
        {
            try
            {
                var result = await twitterService.GetTwitterFollowing(telemetry, username);
                var ob = result as OkObjectResult;
                var value = (decimal)ob.Value;
                if (value > 0)
                {
                    return await Chart.SaveData(value, MetricType.TwitterFollowers, username);
                }

                return NoContent();
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                throw;
            }
        }
    }
}
