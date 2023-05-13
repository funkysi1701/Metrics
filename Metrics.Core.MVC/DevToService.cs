using Metrics.Core.Service;
using Metrics.Model;
using Metrics.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Metrics.Core.MVC
{
    public class DevToService
    {
        private readonly MongoDataService Chart;
        private IConfiguration Configuration { get; set; }

        public DevToService(IConfiguration configuration, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new MongoDataService(mongoService);
        }

        public async Task<IActionResult> GetOps(string username)
        {
            IActionResult result = null;

            List<BlogPosts> blogs = new();
            for (int i = 0; i < 100; i++)
            {
                blogs.AddRange(await GetAllBlogs.GetAllOps(Configuration, 10, i));
            }
            result = await Chart.SaveData(blogs.Count, MetricType.OPSPosts, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(blogs.Count(x => x.Published), MetricType.OPSPublishedPosts, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            int views = 0;
            int reactions = 0;
            int comments = 0;
            foreach (var item in blogs)
            {
                views += item.Page_Views_Count;
                reactions += item.Positive_Reactions_Count;
                comments += item.Comments_Count;
            }
            result = await Chart.SaveData(views, MetricType.OPSViews, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(reactions, MetricType.OPSReactions, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(comments, MetricType.OPSComments, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }

        public async Task<IActionResult> GetDevTo(string username, IHttpClientFactory factory)
        {
            IActionResult result = null;

            var blogs = await GetAllBlogs.GetAll(Configuration, 200, factory);
            result = await Chart.SaveData(blogs.Count, MetricType.DevToPosts, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(blogs.Count(x => x.Published), MetricType.DevToPublishedPosts, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            int views = 0;
            int reactions = 0;
            int comments = 0;
            foreach (var item in blogs)
            {
                views += item.Page_Views_Count;
                reactions += item.Positive_Reactions_Count;
                comments += item.Comments_Count;
            }
            result = await Chart.SaveData(views, MetricType.DevToViews, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(reactions, MetricType.DevToReactions, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(comments, MetricType.DevToComments, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }

            return result;
        }
    }
}
