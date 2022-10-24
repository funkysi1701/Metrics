using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Metrics.Core.Model;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using System;
using Metrics.Core.Enum;
using System.Net.Http;

namespace Metrics.StaticFunction
{
    public class Data
    {
        private readonly IConfiguration Configuration;

        public Data(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [FunctionName("GetData")]
        [OpenApiOperation(operationId: "GetData", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = true, Type = typeof(DateTime), Description = "The **date** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IList<Metric>), Description = "The OK response")]
        public async Task<IActionResult> GetData(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            MetricType type = (MetricType)int.Parse(req.Query["type"]);
            string username = req.Query["username"];
            DateTime date = DateTime.Parse(req.Query["date"]);
            log.LogInformation($"GetChart, type: {type}, username: {username}");
            try
            {
                var result = await GetDetails(type, username, log, date);
                if (result == null)
                {
                    log.LogError("Null Error in Data::GetData");
                }
                log.LogInformation($"GetData OK");
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogError($"Exception {ex.Message} in Data::GetData");
                return new BadRequestResult();
            }
        }

        private async Task<IList<Metric>> GetDetails(MetricType type, string username, ILogger log, DateTime date)
        {
            var client = CreateClient();
            var typeParameter = (int)type;
            using var httpResponse = await client.GetAsync($"{client.BaseAddress}api/GetRawData?type={typeParameter}&username={username}&PageSize={Configuration.GetValue<int>("MaxRecords")}&date={date}");
            string result = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                log.LogError($"Error {result} for GetRawData {typeParameter}");
                return null;
            }
            return JsonConvert.DeserializeObject<List<Metric>>(result);
        }

        public HttpClient CreateClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("FunctionAPI")),
            };
        }
    }
}
