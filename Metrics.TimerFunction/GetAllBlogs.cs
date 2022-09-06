using Metrics.Core.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Metrics.TimerFunction
{
    public static class GetAllBlogs
    {
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

        public static async Task<List<BlogPosts>> GetAllOps(IConfiguration config, int per_page, int page)
        {
            var Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("api-key", config.GetValue<string>("OPSAPI"));
            var productValue = new ProductInfoHeaderValue("Funkysi1701Blog", "1.0");
            var commentValue = new ProductInfoHeaderValue("(+https://www.funkysi1701.com)");

            Client.DefaultRequestHeaders.UserAgent.Add(productValue);
            Client.DefaultRequestHeaders.UserAgent.Add(commentValue);
            var baseurl = config.GetValue<string>("OPSURL");
            using HttpResponseMessage httpResponse = await Client.GetAsync(new Uri($"{baseurl}articles/me/all?per_page={per_page}&page={page}"));
            string result = await httpResponse.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<List<BlogPosts>>(result);
            return posts.Where(x => x.Published).ToList();
        }
    }
}
