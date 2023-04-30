using HtmlAgilityPack;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var data = ParseHtmlUsingHtmlAgilityPack(html);
                log.LogInformation("{Count} {username}", Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Followers")), username);
                return await Chart.SaveData(Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Followers")), 0, username);
            }
            catch (Exception e)
            {
                log.LogError("Failed to save for {username} Exception {Message}", username, e.Message);
                return new BadRequestObjectResult(e.Message);
            }
        }

        private static List<(string RepositoryName, string Description)> ParseHtmlUsingHtmlAgilityPack(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var repositories =
                htmlDoc
                    .DocumentNode
                    .SelectNodes("//div/div/div/div/main/div/div/div/div/div/div/div/div/div/div/div");

            List<(string RepositoryName, string Description)> data = new();

            var repo = repositories[6];
            var nodes = repo.SelectNodes("div");
            foreach (var item in nodes)
            {
                var values = item?.InnerText.Split(" ");
                data.Add((values[1], values[0]));
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
