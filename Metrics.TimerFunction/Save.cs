using Metrics.Core;
using Metrics.TimerFunction.Services;
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
        private readonly PowerService powerService;
        private readonly GithubService githubService;
        private readonly DevToService devToService;
        private readonly BlogService blogService;
        private readonly IConfiguration Configuration;
        private readonly List<string> users;

        public Save(TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService, IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1")
            };
            this.twitterService = twitterService;
            this.powerService = powerService;
            this.githubService = githubService;
            this.devToService = devToService;
            this.blogService = blogService;
        }

        [FunctionName("SaveTwitterFav")]
        public async Task Run1([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFav(log);
        }

        [FunctionName("SaveTwitterFollowers")]
        public async Task Run2([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFollowers(log);
        }

        [FunctionName("SaveTwitterFollowing")]
        public async Task Run3([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetTwitterFollowing(log);
        }

        [FunctionName("SaveNumberOfTweets")]
        public async Task Run4([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await twitterService.GetNumberOfTweets(log);
        }

        [FunctionName("SaveGas")]
        public async Task Run5([TimerTrigger("0 59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await powerService.GetGas();
        }

        [FunctionName("SaveElec")]
        public async Task Run6([TimerTrigger("0 59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await powerService.GetElec();
        }

        [FunctionName("SaveCommits")]
        public async Task Run7([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            foreach (var username in users)
            {
                var result = await githubService.GetCommits(username);
                try
                {
                    var okMessage = (OkObjectResult)result;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = (BadRequestObjectResult)result;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        [FunctionName("SaveGitHubFollowers")]
        public async Task Run8([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            foreach (var username in users)
            {
                var result = await githubService.GetGitHubFollowers(username);
                try
                {
                    var okMessage = (OkObjectResult)result;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = (BadRequestObjectResult)result;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        [FunctionName("SaveGitHubFollowing")]
        public async Task Run9([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            foreach (var username in users)
            {
                var result = await githubService.GetGitHubFollowing(username);
                try
                {
                    var okMessage = (OkObjectResult)result;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = (BadRequestObjectResult)result;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        [FunctionName("SaveGitHubRepo")]
        public async Task Run10([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            foreach (var username in users)
            {
                var result = await githubService.GetGitHubRepo(username);
                try
                {
                    var okMessage = (OkObjectResult)result;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = (BadRequestObjectResult)result;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        [FunctionName("SaveGitHubStars")]
        public async Task Run11([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            foreach (var username in users)
            {
                var result = await githubService.GetGitHubStars(username);
                try
                {
                    var okMessage = (OkObjectResult)result;
                    log.LogInformation(okMessage.Value.ToString());
                }
                catch (Exception e)
                {
                    log.LogError(e.Message);
                    var badMessage = (BadRequestObjectResult)result;
                    log.LogError(badMessage.Value.ToString());
                    throw;
                }
            }
        }

        [FunctionName("SaveDevTo")]
        public async Task Run12([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            foreach (var username in users)
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
            var feedList = new List<SaveBlog>
            {
                new SaveBlog() { Feed = Configuration.GetValue<string>("RSSFeed"), Type = (int)MetricType.Blog },
                new SaveBlog() { Feed = Configuration.GetValue<string>("OldRSSFeed"), Type = (int)MetricType.OldBlog }
            };

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
