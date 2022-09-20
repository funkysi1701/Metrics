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

        protected override async Task OnInitializedAsync()
        {
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

        public void RefreshMe()
        {
            StateHasChanged();
        }
    }
}
