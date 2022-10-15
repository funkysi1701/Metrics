﻿using Metrics.Core.Service;
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
            IActionResult result = await Chart.SaveData(user.Followers, 4, username);
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
            IActionResult result = await Chart.SaveData(user.Following, 5, username);
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
            IActionResult result = await Chart.SaveData(user.PublicRepos, 6, username);
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
            IActionResult result = await Chart.SaveData(stars.Count, 7, username);
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
            var sofar = await Chart.Get(8, username);
            sofar = sofar.Where(x => x.Date != null && x.Date < DateTime.Now.Date).OrderBy(y => y.Date).ToList();
            if (sofar.Count == 0)
            {
                result = await Chart.SaveData(today.Count, 8, username);
                try
                {
                    _ = result as OkObjectResult;
                }
                catch
                {
                    return result;
                }
            }
            else result = await Chart.SaveData(today.Count + sofar.Last().Value.Value, 8, username);
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
