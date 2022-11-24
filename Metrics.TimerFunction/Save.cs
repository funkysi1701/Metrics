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

        public Save(TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService, IConfiguration Configuration, MastodonService mastodonService)
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
        }

        [FunctionName("SaveTwitterFollowers")]
        public async Task Run2([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveMastodonFollowers")]
        public async Task Run14([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveMastodonFollowing")]
        public async Task Run15([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveMastodonToots")]
        public async Task Run17([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveTwitterFollowing")]
        public async Task Run3([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveNumberOfTweets")]
        public async Task Run4([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveGas")]
        public async Task Run5([TimerTrigger("0 59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveElec")]
        public async Task Run6([TimerTrigger("0 59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveCommits")]
        public async Task Run7([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveGitHubFollowers")]
        public async Task Run8([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveGitHubFollowing")]
        public async Task Run9([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveGitHubRepo")]
        public async Task Run10([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveGitHubStars")]
        public async Task Run11([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveDevTo")]
        public async Task Run12([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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

        [FunctionName("SaveBlog")]
        public async Task Run13([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
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
    }
}
