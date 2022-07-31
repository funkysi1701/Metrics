using Metrics.Core;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metrics.TimerFunction.Services
{
    public class MongoService
    {
        private readonly IMongoCollection<Metric> _collection;

        public MongoService(IOptions<MyMongoDatabaseSettings> DatabaseSettings)
        {
            if (DatabaseSettings == null)
            {
                throw new ArgumentNullException(nameof(DatabaseSettings));
            }
            var mongoClient = new MongoClient(
                DatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                DatabaseSettings.Value.DatabaseName);

            _collection = mongoDatabase.GetCollection<Metric>(
                DatabaseSettings.Value.CollectionName);
        }

        public async Task CreateAsync(Metric metric) =>
            await _collection.InsertOneAsync(metric);

        public async Task UpdateAsync(string id, Metric updatedBook) =>
            await _collection.ReplaceOneAsync(x => x.id == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.id == id);

        public async Task<List<Metric>> GetAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<Metric?> GetAsync(string id) =>
            await _collection.Find(x => x.id == id).FirstOrDefaultAsync();
    }
}
