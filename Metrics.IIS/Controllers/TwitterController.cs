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
        private readonly string username;

        public TwitterController(TwitterService twitterService, TelemetryClient telemetry, SaveDataService saveDataService, IConfiguration config)
        {
            this.twitterService = twitterService;
            this.saveDataService = saveDataService;
            this.telemetry = telemetry;
            username = config.GetValue<string>("Username1");
        }

        /// <summary>
        /// SaveTwitterFollowers
        /// </summary>
        /// <remarks>SaveTwitterFollowers</remarks>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SaveTwitterFollowers(int t = 120)
        {
            try
            {
                var result = await twitterService.GetTwitterFollowers(telemetry, username, t);
                if (result is not OkObjectResult ob)
                {
                    return result;
                }

                if (ob == null || ob.Value == null)
                {
                    return NoContent();
                }

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
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(double), 200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> SaveTwitterFollowing(int t = 120)
        {
            try
            {
                var result = await twitterService.GetTwitterFollowing(telemetry, username, t);
                if (result is not OkObjectResult ob)
                {
                    return result;
                }

                if (ob == null || ob.Value == null)
                {
                    return NoContent();
                }

                var value = (decimal)ob.Value;
                if (value > 0)
                {
                    return await saveDataService.SaveData(value, MetricType.TwitterFollowing, username);
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
