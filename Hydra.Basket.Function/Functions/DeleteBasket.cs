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
    public class DeleteBasket
    {
      private readonly MongoClient _mongoClient;
        public readonly ILogger _logger;
        private readonly IConfiguration _config;

         public readonly IMongoCollection<Models.Basket> _collection;

        public DeleteBasket(
            MongoClient mongoClient,
            ILogger<DeleteBasket> logger,
            IConfiguration config)
        {
            _mongoClient = mongoClient;
            _logger = logger;
            _config = config;

            var database = _mongoClient.GetDatabase("HydraBasket");
            _collection = database.GetCollection<Models.Basket>("Basket");
        }

        [FunctionName("DeleteBasket")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete")] HttpRequest req,
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage)
        {
            IActionResult returnValue = null;
            Guid userId = Guid.Parse(req.QueryString.Value.Replace("?UserId=", "")); // TODO

            try
            {
               _collection.DeleteOne(s => s.UserId == userId);

                await signalRMessage.AddAsync(
                                    new SignalRMessage {
                                            Target = "basket",
                                            UserId = userId.ToString(),
                                            Arguments = new[] { JsonConvert.SerializeObject(null) }
                                    });

                returnValue = new OkObjectResult(null);
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