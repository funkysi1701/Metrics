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

        public Save(TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService)
        {
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
            await githubService.GetCommits();
        }

        [FunctionName("SaveGitHubFollowers")]
        public async Task Run8([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubFollowers();
        }

        [FunctionName("SaveGitHubFollowing")]
        public async Task Run9([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubFollowing();
        }

        [FunctionName("SaveGitHubRepo")]
        public async Task Run10([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await githubService.GetGitHubRepo();
        }

        [FunctionName("SaveGitHubStars")]
        public async Task Run11([TimerTrigger("0 59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var result = await githubService.GetGitHubStars();
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

        [FunctionName("SaveDevTo")]
        public async Task Run12([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var result = await devToService.GetDevTo();
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
            result = await devToService.GetOps();
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

        [FunctionName("SaveBlog")]
        public async Task Run13([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context, IConfiguration Configuration)
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
    }
}
