using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using Hydra.Basket.API.Validations;

namespace Hydra.Basket.API.Models
{
    public class BasketCustomer
    {
        public const int MAX_ITEM_ALLOWED = 15;
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public ValidationResult ValidationResult { get; set; }
       
        public BasketCustomer(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
        }

        public BasketCustomer(){  }

        internal void CalculateTotalPriceBasket() => TotalPrice = Items.Sum(s => s.CalculatePrice());

        internal bool ExistingItem(BasketItem item) => Items.Any(a => a.ProductId == item.ProductId);

        internal BasketItem GetItemByProductId(Guid productId) => Items.FirstOrDefault(f => f.ProductId == productId);

        internal void AddItem(BasketItem item)
        {
            item.AddBasket(Id);
        
            if(ExistingItem(item))
            {
                var existingItem = GetItemByProductId(item.ProductId);
                existingItem.AddQty(item.Qty);

                item = existingItem;

                Items.Remove(existingItem);
            }

            Items.Add(item);

            CalculateTotalPriceBasket();
        }

        internal void UpdateItem(BasketItem item)
        {
            var existingItem = GetItemByProductId(item.ProductId);
            Items.Remove(existingItem);
            Items.Add(item);
            
            CalculateTotalPriceBasket();
        }

        internal void UpdateQuantity(BasketItem item, int qty)
        {
            item.UpdateQty(qty);
            UpdateItem(item);
        }

        internal void RemoveItem(BasketItem item)
        {
            Items.Remove(GetItemByProductId(item.ProductId));
            CalculateTotalPriceBasket();
        }

        internal bool IsValid()
        {
            var errors = Items.SelectMany(i => new BasketItemValidation().Validate(i).Errors).ToList();
            errors.AddRange(new BasketCustomerValidation().Validate(this).Errors);

            ValidationResult = new ValidationResult(errors);

            return ValidationResult.IsValid;
        }
    }
}