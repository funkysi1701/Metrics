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
    public class GetPaged
    {
        private readonly MongoService _mongoService;

        public GetPaged(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [FunctionName("GetPaged")]
        [OpenApiOperation(operationId: "GetFnPaged", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiParameter(name: "PageSize", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **PageSize** parameter")]
        [OpenApiParameter(name: "PageNum", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **PageNum** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IActionResult), Description = "The OK response")]
        public async Task<IActionResult> GetFnPaged(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                int type = int.Parse(req.Query["type"]);
                string username = req.Query["username"];
                int PageSize = int.Parse(req.Query["PageSize"]);
                int PageNum = int.Parse(req.Query["PageNum"]);
                log.LogInformation($"GetPaged, type: {type}, username: {username}, PageSize: {PageSize}, PageNum: {PageNum}");
                var result = await GetMongo(type, username, PageSize, PageNum);
                return new OkObjectResult(result);
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in GetMongo::GetPaged");
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task<List<Metric>> GetMongo(int type, string username, int PageSize, int PageNum)
        {
            return await _mongoService.GetAsync(type, username, PageSize, PageNum);
        }
    }
}
