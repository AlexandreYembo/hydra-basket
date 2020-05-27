using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydra.Basket.Function.Models
{
    public class Basket
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Update { get; set; }

        public decimal Total { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<BasketItem> Items { get; set; }

        public void UpdateTotal() => Total = Items.Sum(s => s.Price);
    }
}