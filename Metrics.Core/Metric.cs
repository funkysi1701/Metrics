namespace Metrics.Core
{
    public class Metric
    {
        public string id { get; set; }
        public long MetricId { get; set; }
        public string PartitionKey { get; set; }
        public decimal? Value { get; set; }
        public DateTime? Date { get; set; }
        public int? Type { get; set; }
        public string Username { get; set; }
    }
}
