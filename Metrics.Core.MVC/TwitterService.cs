using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Metrics.Core.MVC
{
    public class TwitterService
    {
        private readonly MongoDataService Chart;
        private TwitterClient TwitterClient { get; set; }

        public TwitterService(IConfiguration configuration, MongoService mongoService)
        {
            Chart = new MongoDataService(mongoService);
            TwitterClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
        }

        public async Task<IActionResult> GetTwitterFollowers(ILogger log, string username)
        {
            TwitterClient.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            try
            {
                var count = new List<long>();
                var followers = TwitterClient.Users.GetFollowerIdsIterator(new GetFollowerIdsParameters(username)
                {
                    PageSize = 1000
                });
                while (!followers.Completed)
                {
                    var page = await followers.NextPageAsync();
                    count.AddRange(page);
                }
                log.LogInformation("{Count} {username}", count.Count, username);
                return await Chart.SaveData(count.Count, 0, username);
            }
            catch (Exception e)
            {
                log.LogError("Failed to save for {username} Exception {Message}", username, e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task<IActionResult> GetTwitterFollowing(string username)
        {
            var friends = (await TwitterClient.Users.GetFriendIdsAsync(username)).Length;
            return await Chart.SaveData(friends, 1, username);
        }

        public async Task<IActionResult> GetNumberOfTweets(string username)
        {
            var friends = await TwitterClient.Users.GetUserAsync(username);
            return await Chart.SaveData(friends.StatusesCount, 2, username);
        }
    }
}
