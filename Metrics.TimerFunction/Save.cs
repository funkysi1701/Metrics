using Metrics.TimerFunction.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        private readonly IOptions<MyMongoDatabaseSettings> DatabaseSettings;

        public Save(TwitterService twitterService, PowerService powerService, GithubService githubService, DevToService devToService, BlogService blogService, IOptions<MyMongoDatabaseSettings> DatabaseSettings)
        {
            this.twitterService = twitterService;
            this.powerService = powerService;
            this.githubService = githubService;
            this.devToService = devToService;
            this.blogService = blogService;
            this.DatabaseSettings = DatabaseSettings;
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
            log.LogInformation(DatabaseSettings.Value.DatabaseName);
            log.LogInformation(DatabaseSettings.Value.CollectionName);
            log.LogInformation(DatabaseSettings.Value.ConnectionString);
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
            await githubService.GetGitHubStars();
        }

        [FunctionName("SaveDevTo")]
        public async Task Run12([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await devToService.GetDevTo();
        }

        [FunctionName("SaveBlog")]
        public async Task Run13([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await blogService.GetBlogCount(log);
        }

        [FunctionName("SaveOldBlog")]
        public async Task Run14([TimerTrigger("0 59 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            await blogService.GetOldBlogCount(log);
        }
    }
}
