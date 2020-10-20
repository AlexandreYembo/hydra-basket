using System;
using System.Linq;
using System.Threading.Tasks;
using Hydra.Basket.Domain.Entities;
using Hydra.Basket.Domain.Repositories;
using Hydra.WebAPI.Core.Controllers;
using Hydra.WebAPI.Core.Identity;
using Hydra.WebAPI.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hydra.Basket.API.Controllers
{
    [Authorize]
    public class BasketController : MainController
    {
        private readonly IAspNetUser _user;
        private readonly IBasketRepository _repository;

        public BasketController(IAspNetUser user, IBasketRepository repository)
        {
            _user = user;
            _repository = repository;
        }

        [HttpGet("basket")]
        [ClaimsAuthorize("basket", "read")]
        public async Task<BasketCustomer> GetBasket() =>
            await GetBasketCustomerAsync() ?? new BasketCustomer();

        [HttpPost("basket")]
        [ClaimsAuthorize("basket", "write")]
        public async Task<IActionResult> AddItemBasket(BasketItem item)
        {
           var basket = await GetBasketCustomerAsync();

            if(basket == null)
                await SaveNewBasket(item);
            else
                await UpdateExistingBasket(basket, item); //TODO update

            if(!ValidOperation()) return CustomResponse();
        
            return CustomResponse();
        }

        [HttpPut("basket/{productId}")]
        [ClaimsAuthorize("basket", "write")]
        public async Task<IActionResult> UpdateItemBasket(Guid productId, BasketItem item)
        {
            var basket = await GetBasketCustomerAsync();
            var basketItem = GetBasketItem(productId, basket, item);

            if(basketItem == null) return CustomResponse();

            basket.UpdateQuantity(basketItem, item.Qty);

            ValidateBasket(basket);
            if(!ValidOperation()) return CustomResponse();

            await _repository.Save(basket);

            return CustomResponse();
        }

        [HttpDelete("basket/{productId}")]
        [ClaimsAuthorize("basket", "write")]
        public async Task<IActionResult> DeleteItemBasket(Guid productId)
        {
            var basket = await GetBasketCustomerAsync();
            var basketItem = GetBasketItem(productId, basket);

            if(basketItem == null) return CustomResponse();
        
            ValidateBasket(basket);
            if(!ValidOperation()) return CustomResponse();

            basket.RemoveItem(basketItem);
        
            await _repository.Save(basket);

            return CustomResponse();
        }

        [HttpPost]
        [Route("basket/apply-voucher")]
        public async Task<IActionResult> ApplyVoucher(Voucher voucher)
        {
            var basket = await GetBasketCustomerAsync();

            basket.ApplyVoucher(voucher);

            await _repository.Save(basket);
            return CustomResponse();
        }

        private async Task<BasketCustomer> GetBasketCustomerAsync() =>
             await _repository.GetByCustomerId(_user.GetUserId());

        private async Task SaveNewBasket(BasketItem item)
        {
            var basket = new BasketCustomer(_user.GetUserId());
            basket.AddItem(item);

            if(!ValidateBasket(basket)) return;

            await _repository.Save(basket);
        }
        private async Task UpdateExistingBasket(BasketCustomer basket, BasketItem item)
        {
            basket.AddItem(item);
            
            if(!ValidateBasket(basket)) return;
            await _repository.Save(basket);
        }

        private BasketItem GetBasketItem(Guid productId, BasketCustomer basket, BasketItem item = null)
        {
            if(item != null && productId != item.ProductId)
            {
                AddErrors("Product id is different");
                return null;
            }

            if(basket == null)
            {
                AddErrors("Basket Not found");
                return null;
            }

            var basketItem = basket.Items.FirstOrDefault(f => f.BasketId == basket.Id && f.ProductId == productId);

            if(basketItem == null || !basket.ExistingItem(basketItem))
            {
                AddErrors("Item was not added to the basket");
                return null;
            }

            return basketItem;
        }

        private bool ValidateBasket(BasketCustomer basket)
        {
            if(basket.IsValid()) return true;

            basket.ValidationResult.Errors.ToList().ForEach(e => AddErrors(e.ErrorMessage));
            return false;
        }
    }
}