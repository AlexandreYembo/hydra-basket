using System;
using FluentValidation;
using Hydra.Basket.Domain.Entities;

namespace Hydra.Basket.Domain.Validations
{
    public class BasketCustomerValidation : AbstractValidator<BasketCustomer>
    {
        public BasketCustomerValidation()
        {
            RuleFor(c => c.CustomerId)
                .NotEqual(Guid.Empty)
                .WithMessage("Customer not found");

            RuleFor(c => c.Items.Count)
                .GreaterThan(0)
                .WithMessage("Basket does not have item");

            RuleFor(c => c.TotalPrice)
                .GreaterThan(0)
                .WithMessage("The total price should be greater than 0");
        }
    }
}