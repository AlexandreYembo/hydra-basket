using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Hydra.Basket.Function.Models
{
    public class Basket
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public DateTime Created { get; set; }
        public DateTime? Update { get; set; }

        [JsonProperty("total")]
        public decimal Total { get; set; }

        [JsonProperty("totalQty")]
        public int TotalQty {get; set; }

        public bool IsActive { get; set; }

        public Guid UserId {get; set;}

        [JsonProperty("items")]
        public List<BasketItem> Items { get; set; }

        public void UpdateTotal(){
            Items.ForEach(s => s.UpdateTotal());
            Total = Items.Sum(s => s.TotalPrice);
            TotalQty = Items.Sum(s => s.Qty);
        } 

        internal List<BrokerRules> Valid()
        {
            List<BrokerRules> rules = new List<BrokerRules>();

            if(Items == null || Items.Count == 0)
                rules.Add(new BrokerRules("It is required to add at least one item for the basket"));
            
            if(Items != null && !Items.Any(s => s.Qty > 0))
                rules.Add(new BrokerRules("It is required to 1 item quantity"));

            return rules;
        }
    }
}