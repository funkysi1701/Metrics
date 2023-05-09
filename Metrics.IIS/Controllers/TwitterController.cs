using Metrics.IIS.Services;
using Microsoft.AspNetCore.Mvc;

namespace Metrics.IIS.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TwitterController : ControllerBase
    {
        private readonly TwitterService twitterService;
        private readonly List<string> twusers;
        public TwitterController(TwitterService twitterService, IConfiguration Configuration)
        {
            this.twitterService = twitterService;
            twusers = new List<string>
            {
                Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701"
            };
        }

        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowers()
        {
            return await GetTwitterFollowers();
        }

        [HttpGet]
        public async Task<IActionResult> SaveTwitterFollowing()
        {
            return await GetTwitterFollowing();
        }

        private async Task<IActionResult> GetTwitterFollowers()
        {
            var result = await twitterService.GetTwitterFollowers(null, twusers.FirstOrDefault());
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
            var result = await twitterService.GetTwitterFollowing(null, twusers.FirstOrDefault());
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