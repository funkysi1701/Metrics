using Mastodon.Api;
using Mastodon.Model;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Metrics.Core.MVC
{
    public class MastodonService
    {
        private readonly MongoDataService Chart;

        public MastodonService(IConfiguration configuration, MongoService mongoService)
        {
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetMastodonFollowers(ILogger log, string username)
        {
            var domain = "hachyderm.io";
            var clientName = "Mastodon.Net";
            var userName = "funkysi1701@gmail.com";
            var password = "VJvBVHVDEsZzZvZL";

            var oauth = await Apps.Register(domain, clientName, scopes: new[] { Scope.Read, Scope.Write, Scope.Follow });
            var token = await OAuth.GetAccessTokenByPassword(domain, oauth.ClientId, oauth.ClientSecret, userName, password, Scope.Read, Scope.Write, Scope.Follow);

            var user = await Accounts.VerifyCredentials(domain, token.AccessToken);
            var followers = await Accounts.Followers(domain, token.AccessToken, user.Id);

            log.LogInformation("{Count} {username}", followers.Count, username);
            return await Chart.SaveData(followers.Count, 0, username);
        }
    }
}
