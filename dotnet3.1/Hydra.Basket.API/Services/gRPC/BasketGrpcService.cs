using System.Threading.Tasks;
using Grpc.Core;
using Hydra.Basket.Domain.Entities;
using Hydra.Basket.Domain.Repositories;
using Hydra.WebAPI.Core.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Hydra.Basket.API.Services.gRPC
{
    /// <summary>
    /// Service GRPC
    /// </summary>
    [Authorize]
    public class BasketGrpcService : Basket.BasketBase
    {
        private readonly ILogger<BasketGrpcService> _logger;
        private readonly IAspNetUser _aspnetUser;
        private readonly IBasketRepository _repository;

        public BasketGrpcService(ILogger<BasketGrpcService> logger, IAspNetUser aspnetUser, IBasketRepository repository)
        {
            _logger = logger;
            _aspnetUser = aspnetUser;
            _repository = repository;
        }


        public override async Task<BasketCustomerResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Calling GetBasket");

            var basket = await GetBasketCustomer() ?? new BasketCustomer();

            return MapBasketCustomerResponse(basket);
        }

        private async Task<BasketCustomer> GetBasketCustomer() =>
            await _repository.GetByCustomerId(_aspnetUser.GetUserId());

        private BasketCustomerResponse MapBasketCustomerResponse(BasketCustomer basket)
        {
            var basketProto = new BasketCustomerResponse
            {
                Id = basket.Id.ToString(),
                Customerid = basket.CustomerId.ToString(),
                Discount = (double)basket.Discount,
                Hasvoucher = basket.HasVoucher
            };

            if(basket.Voucher != null)
            {
                basketProto.Voucher = new VoucherResponse
                {
                    Code = basket.Voucher.Code,
                    Discount = (double?)basket.Voucher.Discount ?? 0,
                    Discounttype = (int)basket.Voucher.DiscountType
                };
            }

            foreach (var item in basket.Items)
            {
                basketProto.Items.Add(new BasketItemResponse
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Image = item.Image,
                    Productdd = item.ProductId.ToString(),
                    Qty = item.Qty,
                    Price = (double)item.Price
                });
            }

            return basketProto;
        }
    }
}