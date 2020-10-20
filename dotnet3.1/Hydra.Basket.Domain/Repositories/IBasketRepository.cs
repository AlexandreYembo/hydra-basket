using System;
using System.Threading.Tasks;
using Hydra.Basket.Domain.Entities;

namespace Hydra.Basket.Domain.Repositories
{
    public interface IBasketRepository
    {
         Task Save(BasketCustomer entity);
         Task Delete(Guid customerId);
         Task<BasketCustomer> GetByCustomerId(Guid customerId);
    }
}