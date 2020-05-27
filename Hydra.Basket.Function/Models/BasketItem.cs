using System;

namespace Hydra.Basket.Function.Models
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public DateTime Added { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }
}