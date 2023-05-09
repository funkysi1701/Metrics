﻿using HtmlAgilityPack;
using Metrics.Core.MVC;
using Metrics.Core.Service;
using Metrics.Model.Enum;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;

namespace Metrics.IIS.Services
{
    public class TwitterService
    {
        private readonly MongoDataService Chart;

        public TwitterService(MongoService mongoService)
        {
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetTwitterFollowers(TelemetryClient telemetry, string username)
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
                        value = Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Followers").Description);
                    }
                    catch (Exception ex)
                    {
                        telemetry.TrackException(ex);
                    }
                }

                telemetry?.TrackEvent($"{value} {username}");
                return await Chart.SaveData(value, MetricType.TwitterFollowers, username);
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task<IActionResult> GetTwitterFollowing(TelemetryClient telemetry, string username)
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
                        value = Convert.ToDecimal(data.FirstOrDefault(x => x.RepositoryName == "Following").Description);
                    }
                    catch (Exception ex)
                    {
                        telemetry.TrackException(ex);
                    }
                }

                telemetry?.TrackEvent($"{value} {username}");
                return await Chart.SaveData(value, MetricType.TwitterFollowing, username);
            }
            catch (Exception e)
            {
                telemetry.TrackException(e);
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
                int result;
                if (int.TryParse(repositories[1].InnerText.Replace(",", ""), out result))
                    data.Add(("Following", result.ToString()));
                if (int.TryParse(repositories[2].InnerText.Replace(",", ""), out result))
                    data.Add(("Following", result.ToString()));
                if (int.TryParse(repositories[3].InnerText.Replace(",", ""), out result))
                    data.Add(("Followers", result.ToString()));
                if (int.TryParse(repositories[4].InnerText.Replace(",", ""), out result))
                    data.Add(("Followers", result.ToString()));
            }

            return data;
        }

        private static string GetHtml(string username)
        {
            var options = new ChromeOptions
            {
            };

            options.AddArguments("headless");

            var chrome = new ChromeDriver(options);
            chrome.Navigate().GoToUrl($"https://twitter.com/{username}");

            return chrome.PageSource;
        }
    }
}
