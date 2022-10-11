using BenchmarkDotNet.Attributes;
using Metrics.Core.Model;
using Metrics.Core.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Metrics.Benchmark
{
    [MemoryDiagnoser(false)]
    public class BenchMark
    {
        private int[] ItemsArray;
        private MyMongoDatabaseSettings opt;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(420);
            var randomItems = Enumerable.Range(0, 1000).Select(_ => random.Next());
            ItemsArray = randomItems.ToArray();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            opt = new MyMongoDatabaseSettings
            {
                ConnectionString = config["ConnectionString"],
                DatabaseName = config["DatabaseName"],
                CollectionName = config["CollectionName"]
            };
        }

        [Benchmark]
        public void For_Array()
        {
            for (int i = 0; i < ItemsArray.Length; i++)
            {
                var item = ItemsArray[i];
            }
        }

        [Benchmark]
        public void ForEach_Array()
        {
            foreach (var item in ItemsArray)
            {

            }
        }

        [Benchmark]
        public async Task Mongo_GetAsync()
        {
            var iopt = Options.Create(opt);
            var m = new MongoService(iopt);
            _ = await m.GetAsync();
        }
    }
}
