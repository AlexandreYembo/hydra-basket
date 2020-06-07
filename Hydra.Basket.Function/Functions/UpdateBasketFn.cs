using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Hydra.Basket.Function.Functions
{
    public class UpdateBasketFn
    {
        private readonly MongoClient _mongoClient;
        public readonly ILogger _logger;
        private readonly IConfiguration _config;

         public readonly IMongoCollection<Models.Basket> _collection;

        public UpdateBasketFn(
            MongoClient mongoClient,
            ILogger<UpdateBasketFn> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("HydraBasket");
            _collection = database.GetCollection<Models.Basket>("Basket");
        }

        [FunctionName("UpdateBasket")]
        public async Task<IActionResult> UpdateBasket(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route= nameof(UpdateBasket) + "/{userId}")] HttpRequest req,
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage,
            string userId)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Models.Basket>(requestBody);

            List<Models.BrokerRules> invalids = input.Valid();

            if(invalids.Count > 0){
                return new BadRequestObjectResult(JsonConvert.SerializeObject(invalids));
            }

            var basket = new Models.Basket
            {
                Id = input.Id,
                Created = DateTime.Now,
                IsActive = input.IsActive,
                Items = input.Items,
                UserId = Guid.Parse(userId)
            };

            basket.UpdateTotal();

            try
            {
               _collection.ReplaceOne(s => s.UserId == Guid.Parse(userId) && s.Id == input.Id, basket);

               await signalRMessage.AddAsync(
                                    new SignalRMessage {
                                            Target = "basket",
                                            UserId = userId,
                                            Arguments = new[] { JsonConvert.SerializeObject(basket) }
                                    });
            return new OkResult();
             //returnValue = new OkObjectResult(basket);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
                //returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                 
            }


          //  return returnValue;
        }
    }
}