using System;
using System.Collections.Generic;

namespace Hydra.Basket.API.Models
{
    public class BasketCustomer
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid TotalPrice { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
       
       public BasketCustomer(Guid customerId)
       {
           Id = Guid.NewGuid();
           CustomerId = customerId;
       }

       public BasketCustomer(){  }
    }
}