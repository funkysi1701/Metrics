using HtmlAgilityPack;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;

namespace Metrics.Core.MVC
{
    public class TwitterService
    {
        private readonly MongoDataService Chart;

        public TwitterService(MongoService mongoService)
        {
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetTwitterFollowers(ILogger log, string username)
        {
            try
            {
                var html = GetHtml(username);
                var data = ParseHtmlUsingHtmlAgilityPack(html, "Followers");
                decimal value = 0;
                if (data != null && data.Count > 0)
                {
                    try
                    {
                        value = Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Followers"));
                    }
                    catch (Exception ex)
                    {
                        value = 0;
                    }
                }

                log.LogInformation("{Count} {username}", value, username);
                return await Chart.SaveData(value, 0, username);
            }
            catch (Exception e)
            {
                log.LogError("Failed to save for {username} Exception {Message}", username, e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task<IActionResult> GetTwitterFollowing(ILogger log, string username)
        {
            try
            {
                var html = GetHtml(username);
                var data = ParseHtmlUsingHtmlAgilityPack(html, "Following");
                decimal value = 0;
                if (data != null && data.Count > 0)
                {
                    try
                    {
                        value = Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Following"));
                    }
                    catch (Exception ex)
                    {
                        value = 0;
                    }
                }

                log.LogInformation("{Count} {username}", value, username);
                return await Chart.SaveData(value, 0, username);
            }
            catch (Exception e)
            {
                log.LogError("Failed to save for {username} Exception {Message}", username, e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }

        private static List<(string RepositoryName, string Description)> ParseHtmlUsingHtmlAgilityPack(string html, string type)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var repositories =
                htmlDoc
                    .DocumentNode
                    .SelectNodes($"//span[contains(text(), {type})]/ancestor::a/span");

            List<(string RepositoryName, string Description)> data = new();

            if (repositories is not null)
            {
                data.Add(("Following", repositories[1].InnerText));
                data.Add(("Followers", repositories[3].InnerText));
            }

            return data;
        }

        private static string GetHtml(string username)
        {
            var options = new ChromeOptions
            {
                BinaryLocation = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
            };

            options.AddArguments("headless");

            var chrome = new ChromeDriver(options);
            chrome.Navigate().GoToUrl($"https://twitter.com/{username}");

            return chrome.PageSource;
        }
    }
}
