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
using System.Net;
using System.Threading.Tasks;

namespace Metrics.Function
{
    public class Delete
    {
        private readonly MongoService _mongoService;

        public Delete(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [FunctionName("Delete")]
        [OpenApiOperation(operationId: "DeleteFn", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IActionResult), Description = "The OK response")]
        public async Task<IActionResult> DeleteFn(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string Id = req.Query["Id"];
                log.LogInformation($"Delete, Id: {Id}");
                await DeleteMongo(Id);
                return new OkObjectResult("ok");
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in GetMongo::Get");
                return new BadRequestObjectResult(e.Message);
            }
        }

        public async Task DeleteMongo(string Id)
        {
            await _mongoService.RemoveAsync(Id);
        }
    }
}
