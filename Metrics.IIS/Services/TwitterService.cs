using HtmlAgilityPack;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;

namespace Metrics.IIS.Services
{
    public class TwitterService
    {
        public async Task<IActionResult> GetTwitterFollowers(TelemetryClient telemetry, string username, bool headless, bool sandbox, bool timeout, int t)
        {
            try
            {
                var html = GetHtml(username, headless, sandbox, timeout, t);
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

                if (data.Count == 0)
                {
                    return new BadRequestObjectResult("No Data");
                }

                telemetry?.TrackEvent($"{value} {username}");
                return new OkObjectResult(value);
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
                var html = GetHtml(username, true, true, true, 120);
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
                return new OkObjectResult(value);
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

        private static string GetHtml(string username, bool headless, bool sandbox, bool timeout, int t)
        {
            var options = new ChromeOptions
            {
            };
            if (headless)
                options.AddArguments("headless");

            if (sandbox)
                options.AddArguments("--no-sandbox");

            var chrome = new ChromeDriver(options);

            if (timeout)
                chrome.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(t);

            chrome
                .Navigate()
                .GoToUrl($"https://twitter.com/{username}");

            return chrome.PageSource;
        }
    }
}
