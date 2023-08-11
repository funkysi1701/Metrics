using Metrics.Core.MVC;
using Metrics.Core.Service;
using Metrics.Model.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Metrics.Function
{
    public class SaveData
    {
        private readonly MongoDataService Chart;

        public SaveData(MongoService mongoService)
        {
            Chart = new MongoDataService(mongoService);
        }

        [FunctionName("SaveData")]
        [OpenApiOperation(operationId: "SaveDataFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiParameter(name: "value", In = ParameterLocation.Query, Required = true, Type = typeof(decimal), Description = "The **value** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IActionResult), Description = "The OK response")]
        public async Task<IActionResult> SaveDataFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                MetricType type = (MetricType)int.Parse(req.Query["type"]);
                string username = req.Query["username"];
                decimal value = decimal.Parse(req.Query["value"]);
                var result = await Chart.SaveData(value, type, username);
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in SaveData");
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
