using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Metrics.StaticFunction
{
    public class Delete
    {
        private readonly IConfiguration Configuration;

        public Delete(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [FunctionName("Delete")]
        [OpenApiOperation(operationId: "Delete", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "Id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> DeleteAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            string Id = req.Query["Id"];
            try
            {
                if (Configuration.GetValue<bool>("DeleteEnabled"))
                {
                    var result = await DeleteCall(Id);
                    if (result == null)
                    {
                        log.LogError("Null Error in Delete::DeleteAsync");
                    }
                    log.LogInformation($"DeleteAsync OK");
                    return new OkObjectResult(result);
                }
                else
                {
                    log.LogError("Delete disabled");
                    return new OkObjectResult("Delete disabled");
                }
            }
            catch (Exception e)
            {
                log.LogError($"Exception {e.Message} in Delete::DeleteAsync");
                return new BadRequestResult();
            }
        }

        public HttpClient CreateClient()
        {
            return new HttpClient
            {
                BaseAddress = new Uri(Configuration.GetValue<string>("FunctionAPI")),
            };
        }

        private async Task<string> DeleteCall(string Id)
        {
            var client = CreateClient();
            using var httpResponse = await client.DeleteAsync($"{client.BaseAddress}api/Delete?Id={Id}");
            await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                return null;
            }

            return "ok";
        }
    }
}
