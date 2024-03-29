﻿using Metrics.Core.MVC;
using Metrics.Model;
using Metrics.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Metrics.TimerFunction
{
    public class Save
    {
        private readonly MastodonService mastodonService;
        private readonly PowerService powerService;
        private readonly GithubService githubService;
        private readonly DevToService devToService;
        private readonly BlogService blogService;
        private readonly IConfiguration Configuration;
        private readonly List<string> ghusers;
        private readonly IHttpClientFactory httpClientFactory;

        public Save(PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService, IConfiguration Configuration, MastodonService mastodonService, IHttpClientFactory httpClientFactory)
        {
            this.Configuration = Configuration;
            ghusers = new List<string>
            {
                Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701"
            };
            this.powerService = powerService;
            this.githubService = githubService;
            this.devToService = devToService;
            this.blogService = blogService;
            this.mastodonService = mastodonService;
            this.httpClientFactory = httpClientFactory;
        }

        [FunctionName("SaveMastodonFollowers")]
        public async Task SaveMastodonFollowers([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveMastodonFollowing([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveFollowFriday([TimerTrigger("0 39,49,59 12 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveMastodonToots([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveGas([TimerTrigger("0 39,49,59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveElec([TimerTrigger("0 39,49,59 */6 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveCommits([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveGitHubFollowers([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveGitHubFollowing([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveGitHubRepo([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveGitHubStars([TimerTrigger("0 39,49,59 */2 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveDevTo([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
        public async Task SaveBlog([TimerTrigger("0 39,49,59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
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
                feedList.Add(new SaveBlog() { Feed = Configuration.GetValue<string>("RSSFeed"), Type = MetricType.Blog });
                foreach (var item in feedList.Where(x => x.Type == MetricType.Blog))
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
            if (Configuration.GetValue<string>("OldRSSFeed") != string.Empty)
            {
                feedList.Add(new SaveBlog() { Feed = Configuration.GetValue<string>("OldRSSFeed"), Type = MetricType.OldBlog });
                foreach (var item in feedList.Where(x => x.Type == MetricType.OldBlog))
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
