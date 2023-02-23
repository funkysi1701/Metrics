using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Metrics.Function
{
    public class GetRawData
    {
        private readonly MongoService _mongoService;

        public GetRawData(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [FunctionName("GetRawData")]
        [OpenApiOperation(operationId: "GetRawDataFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiParameter(name: "PageSize", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **PageSize** parameter")]
        [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **date** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IActionResult), Description = "The OK response")]
        public async Task<IActionResult> GetRawDataFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                int type = int.Parse(req.Query["type"]);
                string username = req.Query["username"];
                int PageSize = int.Parse(req.Query["PageSize"]);
                DateTime date = DateTime.Parse(req.Query["date"]);
                log.LogInformation($"Get, type: {type}, username: {username}, PageSize: {PageSize}");
                var result = await GetMongo(type, username, PageSize, 0, date);
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in GetMongo::Get");
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task<List<Metric>> GetMongo(int type, string username, int PageSize, int PageNum, DateTime date)
        {
            return await _mongoService.GetAsync(type, username, PageSize, PageNum, date);
        }
    }
}
