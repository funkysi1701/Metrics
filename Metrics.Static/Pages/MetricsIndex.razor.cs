using Microsoft.AspNetCore.Components;

namespace Metrics.Static.Pages
{
    public class MetricsBase : ComponentBase
    {
        [Parameter]
        public int OffSet { get; set; } = 0;

        [Parameter]
        public string Username { get; set; } = "funkysi1701";
    }
}
