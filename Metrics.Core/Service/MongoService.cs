﻿using Metrics.Core.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Metrics.Core.Service
{
    public class MongoService : IMongoService
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

        public async Task<List<Metric>> GetAsync(int? type, string username, int PageSize, int PageNum)
        {
            return await _collection
                .Find(i => i.Type == type && i.Username == username)
                .SortByDescending(x => x.Date)
                .Skip(PageNum * PageSize)
                .Limit(PageSize)
                .ToListAsync();
        }
    }
}
