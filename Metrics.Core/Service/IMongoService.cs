using Metrics.Model;

namespace Metrics.Core.Service
{
    public interface IMongoService
    {
        Task CreateAsync(Metric metric);
        Task RemoveAsync(string id);
        Task<List<Metric>> GetAsync();
        Task<List<Metric>> GetAsync(int? type, string username, int PageSize, int PageNum);
    }
}