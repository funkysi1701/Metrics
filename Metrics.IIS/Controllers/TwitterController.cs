using Metrics.IIS.Services;
using Microsoft.AspNetCore.Mvc;

namespace Metrics.IIS.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TwitterController : ControllerBase
    {
        private readonly TwitterService twitterService;

        public TwitterController(TwitterService twitterService)
        {
            this.twitterService = twitterService;
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
            var result = await twitterService.GetTwitterFollowers(null, "funkysi1701");
            try
            {
                var okMessage = result as OkObjectResult;
                return result;
            }
            catch (Exception e)
            {
                var badMessage = result as BadRequestObjectResult;
                throw;
            }
        }

        private async Task<IActionResult> GetTwitterFollowing()
        {
            var result = await twitterService.GetTwitterFollowing(null, "funkysi1701");
            try
            {
                var okMessage = result as OkObjectResult;
                return result;
            }
            catch (Exception e)
            {
                var badMessage = result as BadRequestObjectResult;
                throw;
            }
        }
    }
}
