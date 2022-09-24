using Metrics.Core.Enum;
using Metrics.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Metrics.StaticFunction
{
    public class Chart
    {
        private readonly IConfiguration Configuration;

        public Chart(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [FunctionName("GetChart")]
        [OpenApiOperation(operationId: "GetChart", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "day", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **day** parameter")]
        [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **offset** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IList<IList<ChartView>>), Description = "The OK response")]
        public async Task<IActionResult> GetChart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            MetricType type = (MetricType)int.Parse(req.Query["type"]);
            MyChartType day = (MyChartType)int.Parse(req.Query["day"]);
            int OffSet = int.Parse(req.Query["offset"]);
            string username = req.Query["username"];
            log.LogInformation($"GetChart, type: {type}, day: {day}, offset: {OffSet}, username: {username}");
            try
            {
                var result = await GetChartDetails(type, day, OffSet, username, log);
                if (result == null)
                {
                    log.LogError("Null Error in Chart::GetChart");
                }
                log.LogInformation($"GetChart OK");
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in Chart::GetChart");
                return new BadRequestResult();
            }
        }

        private async Task<IList<IList<ChartViewWithType>>> GetChartDetails(MetricType type, MyChartType day, int OffSet, string username, ILogger log)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("FunctionAPI")),
            };
            var typeParameter = (int)type;
            using var httpResponse = await client.GetAsync($"{client.BaseAddress}api/Get?type={typeParameter}&username={username}&maxRecords=10000");
            string result = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                log.LogError($"Error {result} for Get {typeParameter}");
                return null;
            }
            var metrics = JsonConvert.DeserializeObject<List<Metric>>(result);
            List<Metric> LiveMetrics;
            List<Metric> PrevMetrics;
            if (type == MetricType.Gas || type == MetricType.Electricity)
            {
                OffSet++;
            }

            if (day == MyChartType.Hourly)
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.AddHours(-24 * (OffSet + 1)) && x.Date <= DateTime.Now.AddHours(-24 * OffSet)).ToList();
                PrevMetrics = metrics.Where(x => x.Date > DateTime.Now.AddHours(-24 * (OffSet + 2)) && x.Date <= DateTime.Now.AddHours(-24 * (OffSet + 1))).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
            else if (day == MyChartType.Daily)
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.Date.AddDays(-14)).ToList();
                PrevMetrics = metrics.Where(x => x.Date <= DateTime.Now.Date.AddDays(-14) && x.Date > DateTime.Now.Date.AddDays(-29)).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
            else
            {
                LiveMetrics = metrics.Where(x => x.Date > DateTime.Now.AddDays(-1 * (DateTime.Now.Day - 1)).Date.AddMonths(-11)).ToList();
                PrevMetrics = metrics.Where(x => x.Date <= DateTime.Now.AddDays(-1 * (DateTime.Now.Day - 1)).Date.AddMonths(-11) && x.Date > DateTime.Now.AddDays(-1 * (DateTime.Now.Day - 1)).Date.AddMonths(-23)).ToList();
                return GetResult(LiveMetrics, PrevMetrics);
            }
        }

        private static IList<IList<ChartViewWithType>> GetResult(List<Metric> metrics, List<Metric> Prevmetrics)
        {
            var result = new List<ChartViewWithType>();
            foreach (var item in metrics.Where(x => x.Date != null))
            {
                var c = new ChartViewWithType
                {
                    Type = item.Type.Value,
                    Date = item.Date.Value,
                    Total = item.Value
                };
                result.Add(c);
            }
            var prevresult = new List<ChartViewWithType>();
            foreach (var previtem in Prevmetrics.Where(x => x.Date != null))
            {
                var c = new ChartViewWithType
                {
                    Type = previtem.Type.Value,
                    Date = previtem.Date.Value,
                    Total = previtem.Value
                };
                prevresult.Add(c);
            }
            var final = new List<IList<ChartViewWithType>>
            {
                result,
                prevresult
            };
            return final;
        }
    }
}
