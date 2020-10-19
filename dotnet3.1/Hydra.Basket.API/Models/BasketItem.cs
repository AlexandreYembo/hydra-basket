using System;
using System.Text.Json.Serialization;
using Hydra.Basket.API.Validations;

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
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public Guid BasketId { get; set; }
       
       [JsonIgnore]
        public BasketCustomer BasketCustomer { get; set; }

        internal void AddBasket(Guid basketId) => BasketId = basketId;

        internal decimal CalculatePrice() => Price * Qty;

        internal void AddQty(int qty) => Qty += qty;

        internal void UpdateQty(int qty) => Qty = qty;

        internal bool IsValid() => new BasketItemValidation().Validate(this).IsValid;
    }
}