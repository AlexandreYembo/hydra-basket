using System;
using System.Linq;
using System.Threading.Tasks;
using Hydra.Basket.API.Data;
using Hydra.Basket.API.Models;
using Hydra.WebAPI.Core.Controllers;
using Hydra.WebAPI.Core.Identity;
using Hydra.WebAPI.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hydra.Basket.API.Controllers
{
    [Authorize]
    public class BasketController : MainController
    {
        private readonly IAspNetUser _user;
        private readonly BasketContext _context;


        public BasketController(IAspNetUser user, BasketContext context)
        {
            _user = user;
            _context = context;
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
                SaveNewBasket(item);
            else
                UpdateExistingBasket(basket, item); //TODO update

            if(!ValidOperation()) return CustomResponse();

            await PersistData();
        
            return CustomResponse();
        }

        [HttpPut("basket/{productId}")]
        [ClaimsAuthorize("basket", "write")]
        public async Task<IActionResult> UpdateItemBasket(Guid productId, BasketItem item)
        {
            var basket = await GetBasketCustomerAsync();
            var basketItem = await GetBasketItem(productId, basket, item);

            if(basketItem == null) return CustomResponse();

            basket.UpdateQuantity(basketItem, item.Qty);

            ValidateBasket(basket);
            if(!ValidOperation()) return CustomResponse();

            _context.BasketItems.Update(basketItem);
            _context.BasketCustomers.Update(basket);
            
            await PersistData();

            return CustomResponse();
        }

        [HttpDelete("basket/{productId}")]
        [ClaimsAuthorize("basket", "write")]
        public async Task<IActionResult> DeleteItemBasket(Guid productId)
        {
            var basket = await GetBasketCustomerAsync();
            var basketItem = await GetBasketItem(productId, basket);

            if(basketItem == null) return CustomResponse();
        
            ValidateBasket(basket);
            if(!ValidOperation()) return CustomResponse();

            basket.RemoveItem(basketItem);
        
            _context.BasketItems.Remove(basketItem);
            _context.BasketCustomers.Update(basket);

            await PersistData();
            return CustomResponse();
        }

        private async Task<BasketCustomer> GetBasketCustomerAsync() =>
             await _context.BasketCustomers
                            .Include(c => c.Items)
                            .FirstOrDefaultAsync(c => c.CustomerId == _user.GetUserId());

        private void SaveNewBasket(BasketItem item)
        {
            var basket = new BasketCustomer(_user.GetUserId());
            basket.AddItem(item);

            ValidateBasket(basket);

            _context.BasketCustomers.Add(basket);
        }
        private void UpdateExistingBasket(BasketCustomer basket, BasketItem item)
        {
            var existingItem = basket.ExistingItem(item);
            basket.AddItem(item);
            
            ValidateBasket(basket);

            if(existingItem){
                _context.BasketItems.Update(basket.GetItemByProductId(item.ProductId));
            }
            else{
                _context.BasketItems.Add(item);
            }

            _context.BasketCustomers.Update(basket);            
        }

        private async Task<BasketItem> GetBasketItem(Guid productId, BasketCustomer basket, BasketItem item = null)
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

            var basketItem = await _context.BasketItems.FirstOrDefaultAsync(f => f.BasketId == basket.Id && f.ProductId == productId);

            if(basketItem == null || !basket.ExistingItem(basketItem))
            {
                AddErrors("Item was not added to the basket");
                return null;
            }

            return basketItem;
        }

        private async Task PersistData()
        {
            var result = await _context.SaveChangesAsync();
            if (result <= 0) AddErrors("There was an error to save the data");
        }

        private bool ValidateBasket(BasketCustomer basket)
        {
            if(basket.IsValid()) return true;

            basket.ValidationResult.Errors.ToList().ForEach(e => AddErrors(e.ErrorMessage));
            return false;
        }
    }
}