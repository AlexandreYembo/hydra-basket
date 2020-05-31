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

namespace Hydra.Basket.Function
{
    public class CreateBasket
    {
        private readonly MongoClient _mongoClient;
        public readonly ILogger _logger;
        private readonly IConfiguration _config;

         public readonly IMongoCollection<Models.Basket> _collection;

        public CreateBasket(
            MongoClient mongoClient,
            ILogger<CreateBasket> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("HydraBasket");
            _collection = database.GetCollection<Models.Basket>("Basket");
        }

        [FunctionName("CreateBasket")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage)
        {
            IActionResult returnValue = null;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            var input = JsonConvert.DeserializeObject<Models.Basket>(requestBody);

            List<Models.BrokerRules> invalids = input.Valid();

            if( invalids.Count > 0){
                return new BadRequestObjectResult(JsonConvert.SerializeObject(invalids));
            }

            var basket = new Models.Basket
            {
                Id = input.Id,
                UserId = input.UserId,
                Created = DateTime.Now,
                IsActive = input.IsActive,
                Items = input.Items
            };

            basket.UpdateTotal();

            try
            {
               _collection.InsertOne(basket);

                await signalRMessage.AddAsync(
                                    new SignalRMessage {
                                            Target = "basket",
                                        //    UserId = basket.UserId.ToString(),
                                            Arguments = new[] { JsonConvert.SerializeObject(basket) }
                                    });

                returnValue = new OkObjectResult(JsonConvert.SerializeObject(basket));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown: {ex.Message}");
                returnValue = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }


            return returnValue;
        }
    }
}