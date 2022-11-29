using Mastodon.Api;
using Mastodon.Model;
using Metrics.Core.Enum;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Linq;

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

        public async Task GetFollowFriday(ILogger log, string username)
        {
            await Setup();
            var listoftoots = await Accounts.Statuses(domain, token.AccessToken, user.Id, limit: 20);
            string result = string.Empty;
            var s = new StringBuilder();
            var acc = new List<string>();
            try
            {
                foreach (var tootId in listoftoots.Select(x => x.Id))
                {
                    var fav = await Statuses.FavouritedBy(domain, tootId);
                    var replies = await Statuses.Context(domain, tootId);
                    var retoot = await Statuses.RebloggedBy(domain, tootId);
                    acc.AddRange(from item in fav
                                 where !acc.Contains(item.AccountName)
                                 select item.AccountName);
                    acc.AddRange(from item in replies.Descendants
                                 where !acc.Contains(item.Account.AccountName)
                                 select item.Account.AccountName);
                    acc.AddRange(from item in retoot
                                 where !acc.Contains(item.AccountName)
                                 select item.AccountName);
                }
                foreach (var item in acc)
                {
                    if (s.Length < 450)
                    {
                        s.Append($"@{item}");
                        s.Append(", ");
                    }
                }

                result = s.ToString();
                result = result.Trim();
                result = result.Remove(result.Length - 1, 1);
                var msg = $"#FollowFriday {result}";
                log.LogInformation("{msg}", msg);
                if (DateTime.UtcNow.DayOfWeek == DayOfWeek.Friday && DateTime.UtcNow.Hour == 12 && configuration.GetValue<bool>("EnableToot"))
                {
                    await Statuses.Posting(domain, token.AccessToken, msg);
                }
            }
            catch (Exception ex)
            {
                log.LogError("FollowFriday Error {Message}", ex.Message);
            }
        }

        public async Task<IActionResult> GetMastodonFavourites(ILogger log, string username)
        {
            await Setup();
            var favs = await Favourites.Fetching(domain, token.AccessToken, user.Id, limit: 100);
            log.LogInformation("{Count} {username}", favs.Count, username);
            return await Chart.SaveData(favs.Count, (int)MetricType.MastodonFavourites, username);
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
