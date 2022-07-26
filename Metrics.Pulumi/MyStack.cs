using Pulumi;
using Pulumi.Azure.AppInsights;
using Pulumi.Azure.AppService;
using Pulumi.Azure.AppService.Inputs;
using Pulumi.Azure.Core;

namespace Metrics.Pulumi
{
    public class MyStack : Stack
    {
        public MyStack()
        {
            var resourceGroup = new ResourceGroup("metrics-pulumi", new ResourceGroupArgs
            {
                Name = "metrics-function-pulumi",
            });
        }
    }
}
