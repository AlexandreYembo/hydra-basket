using System;
using System.Threading.Tasks;
using Hydra.Basket.Function.Authentication;
using Hydra.Basket.Function.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Hydra.Basket.Function.Functions
{
    public class GetBasket
    {
        public readonly ILogger _logger;

        private readonly IMongoBase _mongoBase;

        private readonly IWebJobAuthorizeHelper _authorize;
        public GetBasket(IMongoBase mongoBase, ILogger<GetBasket> logger, IWebJobAuthorizeHelper authorize){
            _mongoBase = mongoBase;
            _logger = logger;
            _authorize = authorize;
        }

        [FunctionName("GetBasket")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage)
        {
            string userId = _authorize.GetUserId(req);
            Models.Basket basket = _mongoBase.Find(Guid.Parse(userId));

            await signalRMessage.AddAsync(
                new SignalRMessage {
                    Target = "basket",
                    UserId = userId.ToString(),
                    Arguments = new[] { JsonConvert.SerializeObject(basket) }
                });

            return new OkObjectResult(basket);
        }   
    }
}