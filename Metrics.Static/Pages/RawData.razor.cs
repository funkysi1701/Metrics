using BlazorApplicationInsights;
using Metrics.Core.Enum;
using Metrics.Core.Model;
using Metrics.Static.Services;
using Microsoft.AspNetCore.Components;

namespace Metrics.Static.Pages
{
    public class RawDataBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }
        [Inject] private IApplicationInsights AppInsights { get; set; }

        protected IList<IList<ChartViewWithType>> chartViews;
        protected List<IList<IList<ChartViewWithType>>> listoflists;
        protected MetricType SelectedType { get; set; }
        protected int SelectedOffset { get; set; }
        protected DateTime SelectedOffsetDate { get; set; }
        protected List<MetricType> Types { get; set; }

        protected override void OnInitialized()
        {
            SelectedOffset = 0;
            Types = new List<MetricType>
            {
                MetricType.TwitterFollowers,
                MetricType.TwitterFollowing,
                MetricType.NumberOfTweets,
                MetricType.TwitterFavourites,
                MetricType.GitHubFollowers,
                MetricType.GitHubFollowing,
                MetricType.GitHubRepo,
                MetricType.GitHubStars,
                MetricType.GitHubCommits,
                MetricType.DevToPosts,
                MetricType.DevToPublishedPosts,
                MetricType.DevToViews,
                MetricType.DevToReactions,
                MetricType.DevToComments,
                MetricType.Gas,
                MetricType.Electricity,
                MetricType.Blog,
                MetricType.OldBlog,
                MetricType.OPSPosts,
                MetricType.OPSPublishedPosts,
                MetricType.OPSViews,
                MetricType.OPSReactions,
                MetricType.OPSComments
            };
        }

        protected async Task Refresh()
        {
            SelectedOffset = (int)(DateTime.UtcNow - SelectedOffsetDate).TotalDays;
            listoflists = new List<IList<IList<ChartViewWithType>>>();
            chartViews = await BlogService.GetChart((int)SelectedType, (int)MyChartType.Hourly, SelectedOffset, "funkysi1701");
            listoflists.Add(chartViews);
            await AppInsights.TrackEvent($"LoadRawData MetricType: {(int)SelectedType}, OffSet: 0, User: funkysi1701");
        }
    }
}
