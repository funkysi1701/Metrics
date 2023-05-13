using Metrics.Model;
using Metrics.Model.Enum;

namespace Metrics.Core.Service
{
    public interface IMongoService
    {
        Task CreateAsync(Metric metric);

        Task RemoveAsync(string id);

        Task<List<Metric>> GetAsync();

        Task<List<Metric>> GetAsync(MetricType type, string username, int PageSize, int PageNum);
    }
}
