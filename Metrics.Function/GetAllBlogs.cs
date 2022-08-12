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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Metrics.Function
{
    public static class GetAllBlogs
    {
        [FunctionName("GetAllBlogs")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "n", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **n** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<BlogPosts>), Description = "The OK response")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("GetAllBlogs");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            string n = req.Query["n"];
            var posts = await GetAll(config, int.Parse(n));
            return new OkObjectResult(posts);
        }

        [FunctionName("GetAllOpsBlogs")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "api" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "n", In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The **n** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<BlogPosts>), Description = "The OK response")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("GetAllOpsBlogs");
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            string n = req.Query["n"];
            var posts = await GetAllOps(config, int.Parse(n));
            return new OkObjectResult(posts);
        }

        public static async Task<List<BlogPosts>> GetAll(IConfiguration config, int n)
        {
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("api-key", config.GetValue<string>("DEVTOAPI"));
            var productValue = new ProductInfoHeaderValue("Funkysi1701Blog", "1.0");
            var commentValue = new ProductInfoHeaderValue("(+https://www.funkysi1701.com)");

            Client.DefaultRequestHeaders.UserAgent.Add(productValue);
            Client.DefaultRequestHeaders.UserAgent.Add(commentValue);
            var baseurl = config.GetValue<string>("DEVTOURL");
            using HttpResponseMessage httpResponse = await Client.GetAsync(new Uri($"{baseurl}articles/me/all?per_page={n}"));
            string result = await httpResponse.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            return posts.Where(x => x.Published).ToList();
        }

        public static async Task<List<BlogPosts>> GetAllOps(IConfiguration config, int n)
        {
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("api-key", config.GetValue<string>("OPSAPI"));
            var productValue = new ProductInfoHeaderValue("Funkysi1701Blog", "1.0");
            var commentValue = new ProductInfoHeaderValue("(+https://www.funkysi1701.com)");

            Client.DefaultRequestHeaders.UserAgent.Add(productValue);
            Client.DefaultRequestHeaders.UserAgent.Add(commentValue);
            var baseurl = config.GetValue<string>("OPSURL");
            using HttpResponseMessage httpResponse = await Client.GetAsync(new Uri($"{baseurl}articles/me/all?per_page={n}"));
            string result = await httpResponse.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            return posts.Where(x => x.Published).ToList();
        }
    }
}
