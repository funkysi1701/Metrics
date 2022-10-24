using BlazorApplicationInsights;
using Metrics.Core.Enum;
using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.AspNetCore.Components;

namespace Metrics.Static.Pages
{
    public class RawDataBase : ComponentBase
    {
        [Inject] private ChartService BlogService { get; set; }
        [Inject] private IApplicationInsights AppInsights { get; set; }

        protected IList<Metric> data;
        protected MetricType SelectedType { get; set; }
        protected DateTime SelectedOffsetDate { get; set; }
        protected List<MetricType> Types { get; set; }

        protected override void OnInitialized()
        {
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
            data = await BlogService.GetData((int)SelectedType, "funkysi1701", SelectedOffsetDate);
            await AppInsights.TrackEvent($"LoadRawData MetricType: {(int)SelectedType}, SelectedOffsetDate: {SelectedOffsetDate}, User: funkysi1701");
        }

        protected async Task Delete(string Id)
        {
            await BlogService.Delete(Id);
            await AppInsights.TrackEvent($"Delete Id: {Id}");
        }
    }
}
