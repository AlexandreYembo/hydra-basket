using System;

namespace Hydra.Basket.API.Models
{
    public class BasketItem
    {
        public BasketItem()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid Name { get; set; }
        public Guid Qty { get; set; }
        public Guid Price { get; set; }
        public Guid Image { get; set; }
        public Guid BasketId { get; set; }
        public BasketCustomer BasketCustomer { get; set; }
    }
}