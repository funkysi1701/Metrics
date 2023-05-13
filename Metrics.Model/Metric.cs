using Metrics.Model.Enum;

namespace Metrics.Model
{
    public class Metric
    {
        public string id { get; set; }
        public long MetricId { get; set; }
        public string PartitionKey { get; set; }
        public decimal? Value { get; set; }
        public DateTime? Date { get; set; }
        public MetricType Type { get; set; }
        public string Username { get; set; }
    }
}
