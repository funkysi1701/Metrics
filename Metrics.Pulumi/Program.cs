using Pulumi;
using System.Threading.Tasks;

namespace Metrics.Pulumi
{
    public class Program
    {
        private static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}