﻿using Mastodon.Api;
using Mastodon.Model;
using Metrics.Core.Enum;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Metrics.Core.MVC
{
    public class MastodonService
    {
        private readonly MongoDataService Chart;
        private readonly IConfiguration configuration;

        public MastodonService(IConfiguration configuration, MongoService mongoService)
        {
            this.configuration = configuration;
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetMastodonFollowers(ILogger log, string username)
        {
            var domain = configuration.GetValue<string>("MastodonServer");
            var clientName = "Mastodon.Net";
            var userName = configuration.GetValue<string>("MastodonUser");
            var password = configuration.GetValue<string>("MastodonPass");

            var oauth = await Apps.Register(domain, clientName, scopes: new[] { Scope.Read, Scope.Write, Scope.Follow });
            var token = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, Scope.Read, Scope.Write, Scope.Follow);

            var user = await Accounts.VerifyCredentials(domain, token.AccessToken);
            var followers = await Accounts.Followers(domain, token.AccessToken, user.Id);

            log.LogInformation("{Count} {username}", followers.Count, username);
            return await Chart.SaveData(followers.Count, (int)MetricType.MastodonFollowers, username);
        }

        public async Task<IActionResult> GetMastodonFollowing(ILogger log, string username)
        {
            var domain = configuration.GetValue<string>("MastodonServer");
            var clientName = "Mastodon.Net";
            var userName = configuration.GetValue<string>("MastodonUser");
            var password = configuration.GetValue<string>("MastodonPass");

            var oauth = await Apps.Register(domain, clientName, scopes: new[] { Scope.Read, Scope.Write, Scope.Follow });
            var token = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, Scope.Read, Scope.Write, Scope.Follow);

            var user = await Accounts.VerifyCredentials(domain, token.AccessToken);
            var following = await Accounts.Following(domain, token.AccessToken, user.Id);

            log.LogInformation("{Count} {username}", following.Count, username);
            return await Chart.SaveData(following.Count, (int)MetricType.MastodonFollowing, username);
        }

        public async Task<IActionResult> GetMastodonFavourites(ILogger log, string username)
        {
            var domain = configuration.GetValue<string>("MastodonServer");
            var clientName = "Mastodon.Net";
            var userName = configuration.GetValue<string>("MastodonUser");
            var password = configuration.GetValue<string>("MastodonPass");

            var oauth = await Apps.Register(domain, clientName, scopes: new[] { Scope.Read, Scope.Write, Scope.Follow });
            var token = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, Scope.Read, Scope.Write, Scope.Follow);

            var user = await Accounts.VerifyCredentials(domain, token.AccessToken);
            var favs = await Favourites.Fetching(domain, token.AccessToken, user.Id);

            log.LogInformation("{Count} {username}", favs.Count, username);
            return await Chart.SaveData(favs.Count, (int)MetricType.MastodonFavourites, username);
        }

        public async Task<IActionResult> GetMastodonToots(ILogger log, string username)
        {
            var domain = configuration.GetValue<string>("MastodonServer");
            var clientName = "Mastodon.Net";
            var userName = configuration.GetValue<string>("MastodonUser");
            var password = configuration.GetValue<string>("MastodonPass");

            var oauth = await Apps.Register(domain, clientName, scopes: new[] { Scope.Read, Scope.Write, Scope.Follow });
            var token = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, Scope.Read, Scope.Write, Scope.Follow);

            var user = await Accounts.VerifyCredentials(domain, token.AccessToken);
            var favs = await Accounts.Statuses(domain, token.AccessToken, user.Id);

            log.LogInformation("{Count} {username}", favs.Count, username);
            return await Chart.SaveData(favs.Count, (int)MetricType.NumberOfToots, username);
        }
    }
}
