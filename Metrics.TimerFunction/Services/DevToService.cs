using Metrics.Core.Enum;
using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metrics.TimerFunction.Services
{
    public class DevToService
    {
        private readonly Chart Chart;
        private IConfiguration Configuration { get; set; }

        public DevToService(IConfiguration configuration, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new Chart(mongoService);
        }

        public async Task<IActionResult> GetOps(string username)
        {
            IActionResult result = null;

            List<BlogPosts> blogs = new();
            for (int i = 0; i < 21; i++)
            {
                blogs.AddRange(await GetAllBlogs.GetAllOps(Configuration, 10, i));
            }
            result = await Chart.SaveData(blogs.Count, (int)MetricType.OPSPosts, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(blogs.Count(x => x.Published), (int)MetricType.OPSPublishedPosts, username);
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
            result = await Chart.SaveData(views, (int)MetricType.OPSViews, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(reactions, (int)MetricType.OPSReactions, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(comments, (int)MetricType.OPSComments, username);
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

        public async Task<IActionResult> GetDevTo(string username)
        {
            IActionResult result = null;

            var blogs = await GetAllBlogs.GetAll(Configuration, 200);
            result = await Chart.SaveData(blogs.Count, (int)MetricType.DevToPosts, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(blogs.Count(x => x.Published), (int)MetricType.DevToPublishedPosts, username);
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
            result = await Chart.SaveData(views, (int)MetricType.DevToViews, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(reactions, (int)MetricType.DevToReactions, username);
            try
            {
                _ = result as OkObjectResult;
            }
            catch
            {
                return result;
            }
            result = await Chart.SaveData(comments, (int)MetricType.DevToComments, username);
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
