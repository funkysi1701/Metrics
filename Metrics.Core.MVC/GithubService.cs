using Metrics.Core.Service;
using Metrics.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace Metrics.Core.MVC
{
    public class GithubService
    {
        private readonly MongoDataService Chart;
        private IConfiguration Configuration { get; set; }

        public GithubService(IConfiguration configuration, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new MongoDataService(mongoService);
        }

        public GitHubClient GitHub()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1") != string.Empty ? Configuration.GetValue<string>("Username1") : "funkysi1701"));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            return github;
        }

        public async Task<IActionResult> GetGitHubFollowers(string username)
        {
            var github = GitHub();
            var user = await github.User.Get(username);
            IActionResult result = await Chart.SaveData(user.Followers, MetricType.GitHubFollowers, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IActionResult> GetGitHubFollowing(string username)
        {
            var github = GitHub();
            var user = await github.User.Get(username);
            IActionResult result = await Chart.SaveData(user.Following, MetricType.GitHubFollowing, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IActionResult> GetGitHubRepo(string username)
        {
            var github = GitHub();
            var user = await github.User.Get(username);
            IActionResult result = await Chart.SaveData(user.PublicRepos, MetricType.GitHubRepo, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IActionResult> GetGitHubStars(string username)
        {
            var github = GitHub();
            var stars = await github.Activity.Starring.GetAllForUser(username);
            IActionResult result = await Chart.SaveData(stars.Count, MetricType.GitHubStars, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IActionResult> GetCommits(string username)
        {
            var github = GitHub();
            IActionResult result;
            var events = await github.Activity.Events.GetAllUserPerformed(username);
            var today = events.Where(x => x.Type == "PushEvent" && x.CreatedAt > DateTime.Now.Date).ToList();
            var sofar = await Chart.Get(MetricType.GitHubCommits, username);
            sofar = sofar.Where(x => x.Date != null && x.Date < DateTime.Now.Date).OrderBy(y => y.Date).ToList();
            if (sofar.Count == 0)
            {
                result = await Chart.SaveData(today.Count, MetricType.GitHubCommits, username);
                try
                {
                    _ = result as OkObjectResult;
                }
                catch
                {
                    return result;
                }
            }
            else result = await Chart.SaveData(today.Count + sofar[sofar.Count-1].Value.Value, MetricType.GitHubCommits, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }
    }
}
