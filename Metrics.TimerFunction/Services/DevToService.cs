using Metrics.Core;
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
        private readonly List<string> users;

        public DevToService(IConfiguration configuration, MongoService mongoService)
        {
            Configuration = configuration;
            Chart = new Chart(mongoService);
            users = new List<string>
            {
                Configuration.GetValue<string>("Username1")
            };
        }

        public async Task GetDevTo()
        {
            foreach (var username in users)
            {
                var blogs = await GetAllBlogs.GetAll(Configuration, 200);
                await Chart.SaveData(blogs.Count, (int)MetricType.DevToPosts, username);
                await Chart.SaveData(blogs.Count(x => x.Published), (int)MetricType.DevToPublishedPosts, username);
                int views = 0;
                int reactions = 0;
                int comments = 0;
                foreach (var item in blogs)
                {
                    views += item.Page_Views_Count;
                    reactions += item.Positive_Reactions_Count;
                    comments += item.Comments_Count;
                }
                await Chart.SaveData(views, (int)MetricType.DevToViews, username);
                await Chart.SaveData(reactions, (int)MetricType.DevToReactions, username);
                await Chart.SaveData(comments, (int)MetricType.DevToComments, username);
                blogs = await GetAllBlogs.GetAllOps(Configuration, 200);
                await Chart.SaveData(blogs.Count, (int)MetricType.OPSPosts, username);
                await Chart.SaveData(blogs.Count(x => x.Published), (int)MetricType.OPSPublishedPosts, username);
                views = 0;
                reactions = 0;
                comments = 0;
                foreach (var item in blogs)
                {
                    views += item.Page_Views_Count;
                    reactions += item.Positive_Reactions_Count;
                    comments += item.Comments_Count;
                }
                await Chart.SaveData(views, (int)MetricType.OPSViews, username);
                await Chart.SaveData(reactions, (int)MetricType.OPSReactions, username);
                await Chart.SaveData(comments, (int)MetricType.OPSComments, username);
            }
        }
    }
}
