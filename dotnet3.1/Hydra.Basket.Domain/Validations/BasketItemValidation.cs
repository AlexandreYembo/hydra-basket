using System;
using FluentValidation;
using Hydra.Basket.Domain.Entities;

namespace Hydra.Basket.Domain.Validations
{
    public class BasketItemValidation : AbstractValidator<BasketItem>
    {
        public BasketItemValidation()
        {
            RuleFor(c => c.ProductId)
                .NotEqual(Guid.Empty)
                .WithMessage("Product ID Invalid");

            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Product Name is required");
            
            RuleFor(c => c.Qty)
                .GreaterThan(0)
                .WithMessage(item => $"Minimum quantity acceptable for  {item.Name} is 1");

            RuleFor(c => c.Qty)
                .LessThan(BasketCustomer.MAX_ITEM_ALLOWED)
                .WithMessage(item => $"Maximum quantity acceptable for {item.Name} is {BasketCustomer.MAX_ITEM_ALLOWED}");

            RuleFor(c => c.Price)
                .GreaterThan(0)
                .WithMessage(item => $"The price of {item.Name} have to be greater than 0");
        }
    }
}