using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Text;
using Tweetinvi;
using Tweetinvi.Parameters;

namespace Metrics.Core.MVC
{
    public class TwitterService
    {
        private readonly MongoDataService Chart;
        private readonly IConfiguration configuration;
        private TwitterClient TwitterClient { get; set; }

        public TwitterService(IConfiguration configuration, MongoService mongoService)
        {
            Chart = new MongoDataService(mongoService);
            this.configuration = configuration;
            TwitterClient = new TwitterClient(configuration.GetValue<string>("TWConsumerKey"), configuration.GetValue<string>("TWConsumerSecret"), configuration.GetValue<string>("TWAccessToken"), configuration.GetValue<string>("TWAccessSecret"));
        }

        public async Task GetFollowFriday(ILogger log)
        {
            var listoftoots = await TwitterClient.Timelines.GetUserTimelineAsync("funkysi1701");
            string result = string.Empty;
            var s = new StringBuilder();
            var acc = new List<FollowFriday>();

            foreach (var tootId in listoftoots.Select(x => x.Id))
            {
                //acc = await CalcFav(tootId, acc);
                //acc = await CalcReplies(tootId, acc);
                //acc = await CalcRetoot(tootId, acc);
            }

            var groupedResults = acc
                .Where(x => !x.Name.Contains("funkysi1701"))
                .GroupBy(x => x.Name)
                .Select(q => new FollowFriday
                {
                    Name = q.Key,
                    Score = q.Sum(x => x.Score),
                })
                .OrderByDescending(y => y.Score);

            foreach (var item in groupedResults)
            {
                if (s.Length < 450)
                {
                    s.Append($"@{item.Name}");
                    s.Append(", ");
                }
            }
            result = s.ToString();
            result = result.Trim();
            result = result.Remove(result.Length - 1, 1);
            var msg = $"#FollowFriday {result}";
            log.LogInformation("{msg}", msg);
            if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday && DateTime.UtcNow.Hour == 12 && configuration.GetValue<bool>("EnableTweet"))
            {
                await TwitterClient.Tweets.PublishTweetAsync(msg);
            }
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
