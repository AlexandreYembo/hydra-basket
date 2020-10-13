using System;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Hydra.Basket.Function.Infrastructure
{
    public interface IMongoBase
    {
        void Insert(Models.Basket basket);
        void Delete(Guid userId);
        Models.Basket Find(Guid userId);
        void Update(Models.Basket basket);
    }

    public class MongoBase : IMongoBase
    {
        private readonly IMongoClient _mongoClient;
        private readonly IConfiguration _config;

        public readonly IMongoCollection<Models.Basket> _collection;

        public MongoBase(IMongoClient mongoClient, IConfiguration config)
        {
            _mongoClient = mongoClient;
            _config = config;

             var database = _mongoClient.GetDatabase("HydraBasket");
            _collection = database.GetCollection<Models.Basket>("Basket");
        }

        public void Insert(Models.Basket basket) =>  _collection.InsertOne(basket);

        public void Delete(Guid userId) => _collection.DeleteOne(p => p.UserId == userId);

        public Models.Basket Find(Guid userId) => _collection.Find(p => p.UserId == userId).FirstOrDefault();

        public void Update(Models.Basket basket) => _collection.ReplaceOne(p => p.UserId == basket.UserId, basket);
    }
}