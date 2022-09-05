using Pulumi;
using System.Threading.Tasks;

namespace Metrics.Pulumi
{
    public static class Program
    {
        private static Task<int> Main() => Deployment.RunAsync<MetricsStack>();
    }
}