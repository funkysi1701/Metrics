using Metrics.IIS.Services;
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

        public TwitterController(TwitterService twitterService, TelemetryClient telemetry)
        {
            this.twitterService = twitterService;
            this.telemetry = telemetry;
        }

        /// <summary>
        /// SaveTwitterFollowers
        /// </summary>
        /// <remarks>SaveTwitterFollowers</remarks>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowers()
        {
            return await GetTwitterFollowers();
        }

        /// <summary>
        /// SaveTwitterFollowing
        /// </summary>
        /// <remarks>SaveTwitterFollowing</remarks>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowing()
        {
            return await GetTwitterFollowing();
        }

        private async Task<IActionResult> GetTwitterFollowers()
        {
            try
            {
                var result = await twitterService.GetTwitterFollowers(telemetry, "funkysi1701");
                return result;
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                throw;
            }
        }

        private async Task<IActionResult> GetTwitterFollowing()
        {
            try
            {
                var result = await twitterService.GetTwitterFollowing(telemetry, "funkysi1701");
                return result;
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                throw;
            }
        }
    }
}
