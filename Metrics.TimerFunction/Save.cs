using Metrics.Core.Enum;
using Metrics.Core.Model;
using Metrics.Core.MVC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly ILogger log;

        public Save(ILogger log, TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService, IConfiguration Configuration, MastodonService mastodonService)
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
            this.log = log;
        }

        [FunctionName("SaveTwitterFollowers")]
        public async Task Run2([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await GetTwitterFollowers();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await GetTwitterFollowers();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await GetTwitterFollowers();
            }
        }

        [FunctionName("SaveMastodonFollowers")]
        public async Task Run14([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveMastodonFollowers();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveMastodonFollowers();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveMastodonFollowers();
            }
        }

        [FunctionName("SaveMastodonFollowing")]
        public async Task Run15([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveMastodonFollowing();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveMastodonFollowing();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveMastodonFollowing();
            }
        }

        [FunctionName("SaveFollowFriday")]
        public async Task Run18([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveFollowFriday();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveFollowFriday();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveFollowFriday();
            }
        }

        [FunctionName("SaveMastodonToots")]
        public async Task Run17([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveMastodonToots();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveMastodonToots();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveMastodonToots();
            }
        }

        [FunctionName("SaveTwitterFollowing")]
        public async Task Run3([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveTwitterFollowing();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveTwitterFollowing();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveTwitterFollowing();
            }
        }

        [FunctionName("SaveNumberOfTweets")]
        public async Task Run4([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveNumberOfTweets();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveNumberOfTweets();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveNumberOfTweets();
            }
        }

        [FunctionName("SaveGas")]
        public async Task Run5([TimerTrigger("0 39,49,59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGas();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGas();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGas();
            }
        }

        [FunctionName("SaveElec")]
        public async Task Run6([TimerTrigger("0 39,49,59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveElec();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveElec();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveElec();
            }
        }

        [FunctionName("SaveCommits")]
        public async Task Run7([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveCommits();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveCommits();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveCommits();
            }
        }

        [FunctionName("SaveGitHubFollowers")]
        public async Task Run8([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubFollowers();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubFollowers();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubFollowers();
            }
        }

        [FunctionName("SaveGitHubFollowing")]
        public async Task Run9([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubFollowing();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubFollowing();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubFollowing();
            }
        }

        [FunctionName("SaveGitHubRepo")]
        public async Task Run10([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubRepo();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubRepo();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubRepo();
            }
        }

        [FunctionName("SaveGitHubStars")]
        public async Task Run11([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveGitHubStars();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveGitHubStars();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveGitHubStars();
            }
        }

        [FunctionName("SaveDevTo")]
        public async Task Run12([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveDevTo();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveDevTo();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveDevTo();
            }
        }

        [FunctionName("SaveBlog")]
        public async Task Run13([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            if (Configuration.GetValue<string>("Env") == "dev" && DateTime.Now.Minute == 39)
            {
                await SaveBlog();
            }
            else if (Configuration.GetValue<string>("Env") == "test" && DateTime.Now.Minute == 49)
            {
                await SaveBlog();
            }
            else if (Configuration.GetValue<string>("Env") == "prod" && DateTime.Now.Minute == 59)
            {
                await SaveBlog();
            }
        }

        private async Task SaveGitHubFollowers()
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

        private async Task SaveGitHubFollowing()
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

        private async Task SaveGitHubRepo()
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

        private async Task SaveGitHubStars()
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

        private async Task SaveDevTo()
        {
            foreach (var username in ghusers)
            {
                var result = await devToService.GetDevTo(username);
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

        private async Task SaveBlog()
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

        private async Task SaveCommits()
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

        private async Task SaveElec()
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

        private async Task SaveGas()
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

        private async Task SaveNumberOfTweets()
        {
            foreach (var user in twusers)
            {
                var result = await twitterService.GetNumberOfTweets(user);
                try
                {
                    if (result is OkObjectResult okMessage)
                    {
                        log.LogInformation(okMessage.Value.ToString());
                    }
                    else if (result is BadRequestObjectResult badMessage)
                    {
                        log.LogError(badMessage.Value.ToString());
                    }
                    else
                    {
                        log.LogError("Unknown result");
                    }
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    if (result is BadRequestObjectResult badMessage)
                    {
                        log.LogError(badMessage.Value.ToString());
                    }
                    else
                    {
                        log.LogError("BadRequestObjectResult is null");
                    }

                    throw;
                }
            }
        }

        private async Task SaveTwitterFollowing()
        {
            foreach (var user in twusers)
            {
                var result = await twitterService.GetTwitterFollowing(user);
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

        private async Task SaveFollowFriday()
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

        private async Task GetTwitterFollowers()
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

        private async Task SaveMastodonFollowing()
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

        private async Task SaveMastodonFollowers()
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

        private async Task SaveMastodonToots()
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
