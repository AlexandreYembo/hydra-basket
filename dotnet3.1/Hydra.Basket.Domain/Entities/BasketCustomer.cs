using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Hydra.Basket.Domain.Validations;

namespace Hydra.Basket.Domain.Entities
{
    public class BasketCustomer
    {
        public const int MAX_ITEM_ALLOWED = 15;
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

       [JsonIgnore]
        public ValidationResult ValidationResult { get; set; }

        public bool HasVoucher { get; set; }
        public decimal Discount { get; set; }

        public Voucher Voucher { get; set; }
       
        public BasketCustomer(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
        }

        public BasketCustomer(){  }

        public void CalculateTotalPriceBasket(){
            TotalPrice = Items.Sum(s => s.CalculatePrice());
            CalculateTotalPriceDiscount();
        } 

        private void CalculateTotalPriceDiscount()
        {
            if(!HasVoucher) return;

            decimal discount = 0;
            var price = TotalPrice;

            if(Voucher.DiscountType == VoucherDiscountType.Percentage)
            {
                if(Voucher.Discount.HasValue)
                {
                    discount = (price * Voucher.Discount.Value) / 100;
                }
            }
            else
            {
                if(Voucher.Discount.HasValue)
                {
                    discount = Voucher.Discount.Value;
                }
            }

            price -= discount;

            TotalPrice = price < 0 ? 0 : price;
            Discount = discount;
        }

        public bool ExistingItem(BasketItem item) => Items.Any(a => a.ProductId == item.ProductId);

        public BasketItem GetItemByProductId(Guid productId) => Items.FirstOrDefault(f => f.ProductId == productId);

        public void AddItem(BasketItem item)
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

        public void UpdateItem(BasketItem item)
        {
            var existingItem = GetItemByProductId(item.ProductId);
            Items.Remove(existingItem);
            Items.Add(item);
            
            CalculateTotalPriceBasket();
        }

        public void UpdateQuantity(BasketItem item, int qty)
        {
            item.UpdateQty(qty);
            UpdateItem(item);
        }

        public void RemoveItem(BasketItem item)
        {
            Items.Remove(GetItemByProductId(item.ProductId));
            CalculateTotalPriceBasket();
        }

        public bool IsValid()
        {
            var errors = Items.SelectMany(i => new BasketItemValidation().Validate(i).Errors).ToList();
            errors.AddRange(new BasketCustomerValidation().Validate(this).Errors);

            ValidationResult = new ValidationResult(errors);

            return ValidationResult.IsValid;
        }

        public void ApplyVoucher(Voucher voucher)
        {
            Voucher = voucher;
            HasVoucher = true;
            CalculateTotalPriceBasket();
        }
    }
}