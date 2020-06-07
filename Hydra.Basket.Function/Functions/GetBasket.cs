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
    public class GetBasket
    {
        private readonly MongoClient _mongoClient;
        public readonly ILogger _logger;
        private readonly IConfiguration _config;

         public readonly IMongoCollection<Models.Basket> _collection;

        public GetBasket(
            MongoClient mongoClient,
            ILogger<GetBasket> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("HydraBasket");
            _collection = database.GetCollection<Models.Basket>("Basket");
        }

        [FunctionName("GetBasket")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage)
        {
            IActionResult returnValue = null;
            Guid userId = Guid.Parse(req.QueryString.Value.Replace("?UserId=", "")); // TODO
            try
            {
              Models.Basket basket = _collection.Find(s => s.UserId == userId).FirstOrDefault();

                await signalRMessage.AddAsync(
                                    new SignalRMessage {
                                            Target = "basket",
                                            UserId = userId.ToString(),
                                            Arguments = new[] { JsonConvert.SerializeObject(basket) }
                                    });

                returnValue = new OkObjectResult(basket);
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