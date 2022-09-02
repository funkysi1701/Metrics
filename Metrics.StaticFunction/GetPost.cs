using Metrics.Core;
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
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Metrics.StaticFunction
{
    public class GetPost
    {
        [FunctionName("GetPost")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **id** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(BlogPostsSingle), Description = "The OK response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("GetPost");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("api-key", config.GetValue<string>("DEVTOAPI"));
            var productValue = new ProductInfoHeaderValue("Funkysi1701Blog", "1.0");
            var commentValue = new ProductInfoHeaderValue("(+https://www.funkysi1701.com)");

            Client.DefaultRequestHeaders.UserAgent.Add(productValue);
            Client.DefaultRequestHeaders.UserAgent.Add(commentValue);
            var baseurl = config.GetValue<string>("DEVTOURL");
            string id = req.Query["id"];
            log.LogInformation(id);
            var post = await Client.GetFromJsonAsync<BlogPostsSingle>(new Uri($"{baseurl}articles/{id}"));

            return new OkObjectResult(post);
        }
    }
}
