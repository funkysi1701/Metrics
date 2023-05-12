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
        private readonly SaveDataService saveDataService;
        private readonly TelemetryClient telemetry;
        private readonly string username = "funkysi1701";

        public TwitterController(TwitterService twitterService, TelemetryClient telemetry, SaveDataService saveDataService)
        {
            this.twitterService = twitterService;
            this.saveDataService = saveDataService;
            this.telemetry = telemetry;
        }

        /// <summary>
        /// SaveTwitterFollowers
        /// </summary>
        /// <remarks>SaveTwitterFollowers</remarks>
        /// <param name="headless"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowers(bool headless, bool sandbox, bool timeout, int t)
        {
            try
            {
                var result = await twitterService.GetTwitterFollowers(telemetry, username, headless, sandbox, timeout, t);
                if (result is BadRequestObjectResult)
                {
                    return new BadRequestObjectResult("Error");
                }
                var ob = result as OkObjectResult;
                var value = (decimal)ob.Value;
                if (value > 0)
                {
                    return await saveDataService.SaveData(value, MetricType.TwitterFollowers, username);
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
                    return await saveDataService.SaveData(value, MetricType.TwitterFollowers, username);
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
