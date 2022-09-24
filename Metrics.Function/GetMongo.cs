using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Http;
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
    public class GetMongo
    {
        private readonly MongoService _mongoService;

        public GetMongo(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [FunctionName("Get")]
        [OpenApiOperation(operationId: "GetFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "type", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **type** parameter")]
        [OpenApiParameter(name: "username", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **username** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<Metric>), Description = "The OK response")]
        public async Task<List<Metric>> GetFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                int type = int.Parse(req.Query["type"]);
                string username = req.Query["username"];
                return await Get(type, username);
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in GetMongo::Get");
                return new List<Metric>();
            }
        }

        public async Task<List<Metric>> Get(int type, string username)
        {
            return await _mongoService.GetAsync(type, username);
        }
    }
}
