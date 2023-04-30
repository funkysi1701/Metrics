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
                var data = ParseHtmlUsingHtmlAgilityPack(html);
                decimal value;
                try
                {
                    value = Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Followers"));
                }
                catch (Exception ex)
                {
                    value = 0;
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

        private static List<(string RepositoryName, string Description)> ParseHtmlUsingHtmlAgilityPack(string html)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var repositories =
                htmlDoc
                    .DocumentNode
                    .SelectNodes("//div[@id=\"react-root\"]/div/div/div[2]/main/div/div/div/div[1]/div/div[3]/div/div/div/div/div[5]");

            List<(string RepositoryName, string Description)> data = new();

            if (repositories is not null)
            {
                var repo = repositories[0];
                var nodes = repo.SelectNodes("div/a");
                foreach (var item in nodes)
                {
                    var values = item?.InnerText.Split(" ");
                    if (values != null)
                    {
                        data.Add((values[1], values[0]));
                    }
                }
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
