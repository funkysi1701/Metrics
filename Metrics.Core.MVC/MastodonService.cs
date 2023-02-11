using Mastodon.Api;
using Mastodon.Model;
using Metrics.Core.Enum;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Linq;
using Metrics.Model;

namespace Metrics.Core.MVC
{
    public class MastodonService
    {
        private readonly MongoDataService Chart;
        private readonly IConfiguration configuration;
        private string domain;
        private Auth token;
        private Account user;

        public MastodonService(IConfiguration configuration, MongoService mongoService)
        {
            this.configuration = configuration;
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetMastodonFollowers(ILogger log, string username)
        {
            await Setup();
            log.LogInformation("{Count} {username}", user.FollowersCount, username);
            return await Chart.SaveData(user.FollowersCount, (int)MetricType.MastodonFollowers, username);
        }

        public async Task<IActionResult> GetMastodonFollowing(ILogger log, string username)
        {
            await Setup();
            log.LogInformation("{Count} {username}", user.FollowingCount, username);
            return await Chart.SaveData(user.FollowingCount, (int)MetricType.MastodonFollowing, username);
        }

        public async Task GetFollowFriday(ILogger log)
        {
            await Setup();
            var listoftoots = await Accounts.Statuses(domain, token.AccessToken, user.Id, limit: 20);
            string result = string.Empty;
            var s = new StringBuilder();
            var acc = new List<FollowFriday>();
            try
            {
                foreach (var tootId in listoftoots.Select(x => x.Id))
                {
                    acc = await CalcFav(tootId, acc);
                    acc = await CalcReplies(tootId, acc);
                    acc = await CalcRetoot(tootId, acc);
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
                if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday && DateTime.UtcNow.Hour == 12 && DateTime.UtcNow.DayOfYear / 7 % 2 == 0 && configuration.GetValue<bool>("EnableToot"))
                {
                    await Statuses.Posting(domain, token.AccessToken, msg);
                }
            }
            catch (Exception ex)
            {
                log.LogError("FollowFriday Error {Message}", ex.Message);
            }
        }

        private async Task<List<FollowFriday>> CalcFav(long tootId, List<FollowFriday> acc)
        {
            var fav = await Statuses.FavouritedBy(domain, tootId);
            var favNames = from item in fav
                           select item.AccountName;
            foreach (var name in favNames)
            {
                var n = new FollowFriday { Name = name, Score = 1.0 };
                acc.Add(n);
            }

            return acc;
        }

        private async Task<List<FollowFriday>> CalcReplies(long tootId, List<FollowFriday> acc)
        {
            var replies = await Statuses.Context(domain, tootId);
            var repliesNames = from item in replies.Descendants
                               select item.Account.AccountName;
            foreach (var name in repliesNames)
            {
                var n = new FollowFriday { Name = name, Score = 1.1 };
                acc.Add(n);
            }

            return acc;
        }

        private async Task<List<FollowFriday>> CalcRetoot(long tootId, List<FollowFriday> acc)
        {
            var retoot = await Statuses.RebloggedBy(domain, tootId);
            var retootNames = from item in retoot
                              select item.AccountName;
            foreach (var name in retootNames)
            {
                var n = new FollowFriday { Name = name, Score = 1.3 };
                acc.Add(n);
            }

            return acc;
        }

        public async Task<IActionResult> GetMastodonToots(ILogger log, string username)
        {
            await Setup();
            log.LogInformation("{Count} {username}", user.StatusesCount, username);
            return await Chart.SaveData(user.StatusesCount, (int)MetricType.NumberOfToots, username);
        }

        private async Task Setup()
        {
            domain = configuration.GetValue<string>("MastodonServer");
            var clientName = "Mastodon.Net";
            var userName = configuration.GetValue<string>("MastodonUser");
            var password = configuration.GetValue<string>("MastodonPass");

            var oauth = await Apps.Register(domain, clientName, scopes: new[] { Scope.Read, Scope.Write, Scope.Follow });
            token = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, Scope.Read, Scope.Write, Scope.Follow);

            user = await Accounts.VerifyCredentials(domain, token.AccessToken);
        }
    }
}
