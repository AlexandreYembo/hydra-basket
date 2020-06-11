using System;
using System.Collections.Generic;
using System.IO;
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

namespace Hydra.Basket.Function
{
    public class CreateBasket
    {
        public readonly ILogger _logger;

        private readonly IMongoBase _mongoBase;

        private readonly IWebJobAuthorizeHelper _authorize;
        public CreateBasket(IMongoBase mongoBase, ILogger<CreateBasket> logger, IWebJobAuthorizeHelper authorize){
            _mongoBase = mongoBase;
            _logger = logger;
            _authorize = authorize;
        }

        [FunctionName("CreateBasket")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage)
        {
            return await this.TryCatch(async() => { 
                    string userId = _authorize.GetUserId(req);

                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                    var input = JsonConvert.DeserializeObject<Models.Basket>(requestBody);

                    List<Models.BrokerRules> invalids = input.Valid();

                    if( invalids.Count > 0){
                        return new BadRequestObjectResult(JsonConvert.SerializeObject(invalids));
                    }

                    var basket = new Models.Basket
                        {
                            Id = input.Id,
                            UserId = Guid.Parse(userId),
                            Created = DateTime.Now,
                            IsActive = input.IsActive,
                            Items = input.Items
                        };

                    basket.UpdateTotal();

                    _mongoBase.Insert(basket);

                    await signalRMessage.AddAsync(
                        new SignalRMessage {
                                                Target = "basket",
                                                UserId = userId,
                                                Arguments = new[] { JsonConvert.SerializeObject(basket) }
                                            });

                    return new OkObjectResult(JsonConvert.SerializeObject(basket));
            }
            , _logger);
        }
    }
}