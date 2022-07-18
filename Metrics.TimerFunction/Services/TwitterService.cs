using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Metrics.TimerFunction.Services
{
    public class TwitterService
    {
        private readonly Chart Chart;
        private TwitterClient TwitterClient { get; set; }
        private IConfiguration Configuration { get; set; }

        private readonly List<string> users;

        public TwitterService(IConfiguration configuration, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new Chart(mongoService);
            TwitterClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1"),
                "zogface",
                "juliankay"
            };
        }

        public async Task GetTwitterFollowers(ILogger log)
        {
            TwitterClient.Config.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;
            foreach (var username in users)
            {
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
                    await Chart.SaveData(count.Count, 0, username);
                    log.LogInformation($"{count.Count} {username}");
                }
                catch (Exception e)
                {
                    log.LogError($"Failed to save for {username} Exception {e.Message}");
                }
            }
        }

        public async Task GetTwitterFollowing(ILogger log)
        {
            foreach (var username in users)
            {
                var friends = (await TwitterClient.Users.GetFriendIdsAsync(username)).Length;
                await Chart.SaveData(friends, 1, username);
            }
        }

        public async Task GetNumberOfTweets(ILogger log)
        {
            foreach (var username in users)
            {
                var friends = await TwitterClient.Users.GetUserAsync(username);
                await Chart.SaveData(friends.StatusesCount, 2, username);
            }
        }

        public async Task GetTwitterFav(ILogger log)
        {
            foreach (var username in users)
            {
                try
                {
                    var friends = await TwitterClient.Users.GetUserAsync(username);
                    await Chart.SaveData(friends.FavoritesCount, 3, username);
                }
                catch (Exception e)
                {
                    log.LogError($"Failed to save for {username} Exception {e.Message}");
                }
            }
        }
    }
}
