using System;
using System.Threading.Tasks;
using Hydra.Basket.Domain.Entities;
using Hydra.Basket.Domain.Repositories;
using Hydra.Basket.Infrastructure.Helpers;
using Microsoft.Extensions.Caching.Distributed;

namespace Hydra.Basket.Infrastructure.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _distributedCache;

        public BasketRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task Save(BasketCustomer entity)
        {
            await _distributedCache.SetAsync($"Basket_{entity.CustomerId}", RedisHelper.SerializeToByte(entity));
        }

        public async Task Delete(Guid customerId)
        {
            await _distributedCache.RemoveAsync($"Basket_{customerId}");
        }

        public async Task<BasketCustomer> GetByCustomerId(Guid customerId)
        {
            var obj = RedisHelper.DeserializeToObject<BasketCustomer>(await _distributedCache.GetAsync($"Basket_{customerId}"));
            return obj;
        }
    }
}