using Metrics.Core.Enum;
using Metrics.Core.MVC;
using Metrics.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Metrics.TimerFunction
{
    public class Save
    {
        private readonly TwitterService twitterService;
        private readonly MastodonService mastodonService;
        private readonly PowerService powerService;
        private readonly GithubService githubService;
        private readonly DevToService devToService;
        private readonly BlogService blogService;
        private readonly IConfiguration Configuration;
        private readonly List<string> ghusers;
        private readonly List<string> twusers;
        private readonly IHttpClientFactory httpClientFactory;

        public Save(TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService, IConfiguration Configuration, MastodonService mastodonService, IHttpClientFactory httpClientFactory)
        {
            this.Configuration = Configuration;
            ghusers = new List<string>
            {
                Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701"
            };
            twusers = new List<string>
            {
                Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701",
                "zogface",
                "juliankay"
            };
            this.twitterService = twitterService;
            this.powerService = powerService;
            this.githubService = githubService;
            this.devToService = devToService;
            this.blogService = blogService;
            this.mastodonService = mastodonService;
            this.httpClientFactory = httpClientFactory;
        }

        [FunctionName("SaveMastodonFollowers")]
        public async Task Run14([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveMastodonFollowers(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveMastodonFollowers(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveMastodonFollowers(log);
            }
        }

        [FunctionName("SaveMastodonFollowing")]
        public async Task Run15([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveMastodonFollowing(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveMastodonFollowing(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveMastodonFollowing(log);
            }
        }

        [FunctionName("SaveFollowFriday")]
        public async Task Run18([TimerTrigger("0 39,49,59 12 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "Dev")
            {
                await SaveFollowFriday(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveFollowFriday(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveFollowFriday(log);
            }
        }

        [FunctionName("SaveMastodonToots")]
        public async Task Run17([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveMastodonToots(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveMastodonToots(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveMastodonToots(log);
            }
        }

        [FunctionName("SaveGas")]
        public async Task Run5([TimerTrigger("0 39,49,59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGas(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGas(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGas(log);
            }
        }

        [FunctionName("SaveElec")]
        public async Task Run6([TimerTrigger("0 39,49,59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveElec(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveElec(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveElec(log);
            }
        }

        [FunctionName("SaveCommits")]
        public async Task Run7([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveCommits(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveCommits(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveCommits(log);
            }
        }

        [FunctionName("SaveGitHubFollowers")]
        public async Task Run8([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubFollowers(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubFollowers(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubFollowers(log);
            }
        }

        [FunctionName("SaveGitHubFollowing")]
        public async Task Run9([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubFollowing(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubFollowing(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubFollowing(log);
            }
        }

        [FunctionName("SaveGitHubRepo")]
        public async Task Run10([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubRepo(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubRepo(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubRepo(log);
            }
        }

        [FunctionName("SaveGitHubStars")]
        public async Task Run11([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubStars(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubStars(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubStars(log);
            }
        }

        [FunctionName("SaveDevTo")]
        public async Task Run12([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveDevTo(log, httpClientFactory);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveDevTo(log, httpClientFactory);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveDevTo(log, httpClientFactory);
            }
        }

        [FunctionName("SaveBlog")]
        public async Task Run13([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveBlog(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveBlog(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveBlog(log);
            }
        }

        [FunctionName("SaveTwitterFollowers")]
        public async Task Run2([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await GetTwitterFollowers(log);
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await GetTwitterFollowers(log);
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await GetTwitterFollowers(log);
            }
        }

        private async Task GetTwitterFollowers(ILogger log)
        {
            foreach (var user in twusers)
            {
                var result = await twitterService.GetTwitterFollowers(log, user);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveGitHubFollowers(ILogger log)
        {
            foreach (var username in ghusers)
            {
                var result = await githubService.GetGitHubFollowers(username);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveGitHubFollowing(ILogger log)
        {
            foreach (var username in ghusers)
            {
                var result = await githubService.GetGitHubFollowing(username);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveGitHubRepo(ILogger log)
        {
            foreach (var username in ghusers)
            {
                var result = await githubService.GetGitHubRepo(username);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveGitHubStars(ILogger log)
        {
            foreach (var username in ghusers)
            {
                var result = await githubService.GetGitHubStars(username);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveDevTo(ILogger log, IHttpClientFactory factory)
        {
            foreach (var username in ghusers)
            {
                var result = await devToService.GetDevTo(username, factory);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
                result = await devToService.GetOps(username);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveBlog(ILogger log)
        {
            var feedList = new List<SaveBlog>();
            if (Configuration.GetValue<string>("RSSFeed") != string.Empty)
            {
                feedList.Add(new SaveBlog() { Feed = Configuration.GetValue<string>("RSSFeed"), Type = (int)MetricType.Blog });
            }
            if (Configuration.GetValue<string>("OldRSSFeed") != string.Empty)
            {
                feedList.Add(new SaveBlog() { Feed = Configuration.GetValue<string>("OldRSSFeed"), Type = (int)MetricType.OldBlog });
            }

            foreach (var item in feedList)
            {
                var result = await blogService.GetBlogCount(log, item.Feed, item.Type);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveCommits(ILogger log)
        {
            foreach (var username in ghusers)
            {
                var result = await githubService.GetCommits(username);
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveElec(ILogger log)
        {
            try
            {
                await powerService.GetElec();
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                throw;
            }
        }

        private async Task SaveGas(ILogger log)
        {
            try
            {
                await powerService.GetGas();
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                throw;
            }
        }

        private async Task SaveFollowFriday(ILogger log)
        {
            try
            {
                await mastodonService.GetFollowFriday(log);
            }
            catch (Exception e)
            {
                log.LogError($"SaveFollowFriday {e.Message}");
                throw;
            }
        }

        private async Task SaveMastodonFollowing(ILogger log)
        {
            foreach (var user in ghusers)
            {
                IActionResult result;
                try
                {
                    result = await mastodonService.GetMastodonFollowing(log, user);
                }
                catch (Exception e)
                {
                    log.LogError($"SaveMastodonFollowing {e.Message}");
                    throw;
                }
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveMastodonFollowers(ILogger log)
        {
            foreach (var user in ghusers)
            {
                IActionResult result;
                try
                {
                    result = await mastodonService.GetMastodonFollowers(log, user);
                }
                catch (Exception e)
                {
                    log.LogError($"SaveMastodonFollowers {e.Message}");
                    throw;
                }
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        private async Task SaveMastodonToots(ILogger log)
        {
            foreach (var user in ghusers)
            {
                IActionResult result;
                try
                {
                    result = await mastodonService.GetMastodonToots(log, user);
                }
                catch (Exception e)
                {
                    log.LogError($"SaveMastodonToots {e.Message}");
                    throw;
                }
                try
                {
                    var okMessage = result as OkObjectResult;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = result as BadRequestObjectResult;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }
    }
}
