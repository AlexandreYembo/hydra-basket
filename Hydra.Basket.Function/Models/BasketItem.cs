using System;
using Newtonsoft.Json;

namespace Hydra.Basket.Function.Models
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        
        [JsonProperty("itemName")]
        public string ItemName { get; set; }
        public DateTime Added { get; set; }
        
        [JsonProperty("itemPrice")]
        public decimal ItemPrice { get; set; }
        public decimal TotalPrice { get; set; }
        
        [JsonProperty("qty")]
        public int Qty { get; set; }

        public void UpdateTotal()
        {
           this.TotalPrice = this.Qty * this.ItemPrice;
        }
    }
}