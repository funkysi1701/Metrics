using Metrics.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metrics.TimerFunction.Services
{
    public class MongoService
    {
        private readonly IMongoCollection<Metric> _collection;

        public MongoService(IOptions<MyMongoDatabaseSettings> DatabaseSettings)
        {
            var mongoClient = new MongoClient(
                DatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DatabaseSettings.Value.DatabaseName);

            _collection = mongoDatabase.GetCollection<Metric>(
                DatabaseSettings.Value.CollectionName);
        }

        public async Task CreateAsync(Metric metric) =>
            await _collection.InsertOneAsync(metric);

        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.id == id);

        public async Task<List<Metric>> GetAsync() =>
            await _collection.Find(_ => true).ToListAsync();
    }
}
