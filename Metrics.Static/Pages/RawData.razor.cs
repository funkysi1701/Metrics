using BlazorApplicationInsights;
using Metrics.Core.Enum;
using Metrics.Core.Model;
using Metrics.Static.Services;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;

namespace Metrics.Static.Pages
{
    public class RawDataBase : ComponentBase
    {
        [Inject] private BlogService BlogService { get; set; }
        [Inject] private IApplicationInsights AppInsights { get; set; }

        protected IList<IList<ChartViewWithType>> chartViews;
        protected List<IList<IList<ChartViewWithType>>> listoflists;
        protected MetricType SelectedType { get; set; }
        protected List<MetricType> types { get; set; }

        protected override async Task OnInitializedAsync()
        {
            types = new List<MetricType>
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
            await Load();
        }

        private async Task Load()
        {
            listoflists = new List<IList<IList<ChartViewWithType>>>();
            for (int i = 0; i <= (int)MetricType.OPSComments; i++)
            {
                chartViews = await BlogService.GetChart(i, (int)MyChartType.Hourly, 0, "funkysi1701");
                listoflists.Add(chartViews);
                await AppInsights.TrackEvent($"LoadRawData MetricType: {i}, OffSet: 0, User: funkysi1701");
            }
        }

        protected async Task Refresh()
        {
            listoflists = new List<IList<IList<ChartViewWithType>>>();
            chartViews = await BlogService.GetChart((int)SelectedType, (int)MyChartType.Hourly, 0, "funkysi1701");
            listoflists.Add(chartViews);
            await AppInsights.TrackEvent($"LoadRawData MetricType: {(int)SelectedType}, OffSet: 0, User: funkysi1701");
        }

        public void RefreshMe()
        {
            StateHasChanged();
        }
    }
}
