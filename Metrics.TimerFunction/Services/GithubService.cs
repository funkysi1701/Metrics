using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metrics.TimerFunction.Services
{
    public class GithubService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }
        private readonly List<string> users;

        public GithubService(IConfiguration configuration, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new Chart(mongoService);
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1")
            };
        }

        public GitHubClient GitHub()
        {
            var github = new GitHubClient(new ProductHeaderValue(Configuration.GetValue<string>("Username1")));
            var tokenAuth = new Credentials(Configuration.GetValue<string>("GitHubToken"));
            github.Credentials = tokenAuth;
            return github;
        }

        public async Task GetGitHubFollowers()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var user = await github.User.Get(username);
                await Chart.SaveData(user.Followers, 4, username);
            }
        }

        public async Task GetGitHubFollowing()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var user = await github.User.Get(username);
                await Chart.SaveData(user.Following, 5, username);
            }
        }

        public async Task GetGitHubRepo()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var user = await github.User.Get(username);
                await Chart.SaveData(user.PublicRepos, 6, username);
            }
        }

        public async Task<IActionResult> GetGitHubStars()
        {
            var github = GitHub();
            IActionResult result = null;
            foreach (var username in users)
            {
                var stars = await github.Activity.Starring.GetAllForUser(username);
                result = await Chart.SaveData(stars.Count, 7, username);
                try
                {
                    var ok = result as OkObjectResult;
                }
                catch
                {
                    return result;
                }
            }
            return result;
        }

        public async Task GetCommits()
        {
            var github = GitHub();
            foreach (var username in users)
            {
                var events = await github.Activity.Events.GetAllUserPerformed(username);
                var today = events.Where(x => x.Type == "PushEvent" && x.CreatedAt > DateTime.Now.Date).ToList();
                var sofar = await Chart.GetAll();
                sofar = sofar.Where(x => x.Date != null && x.Type == 8 && x.Date < DateTime.Now.Date).OrderBy(y => y.Date).ToList();
                if (sofar.Count == 0)
                {
                    await Chart.SaveData(today.Count, 8, username);
                }
                else await Chart.SaveData(today.Count + sofar.Last().Value.Value, 8, username);
            }
        }
    }
}
