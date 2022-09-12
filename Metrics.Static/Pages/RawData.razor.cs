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

        protected IList<IList<ChartView>> chartViews;

        protected override void OnInitialized()
        {
            _ = Load();
        }

        private Task Load()
        {
            return Task.Run(async () => await Load());

            async Task Load()
            {
                chartViews = await BlogService.GetChart(0, (int)MyChartType.Hourly, 0, "funkysi1701");
                await AppInsights.TrackEvent($"LoadRawData MetricType: 0, OffSet: 0, User: funkysi1701");
            }
        }

        public void RefreshMe()
        {
            StateHasChanged();
        }
    }
}
