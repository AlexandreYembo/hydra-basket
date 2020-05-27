using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Collections.Generic;
using System.Linq;

namespace Hydra.Basket.Function
{
    public static class BasketFunction
    {
        private static Models.Basket _basket = new Models.Basket {
           Id = Guid.NewGuid(),
           Created = DateTime.Now,
           IsActive = true
        };

        [FunctionName("getBasket")]
        public static async Task<ActionResult> GetBasket(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            // [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, >>> Implement once there is a queue working behind
            [SignalR(HubName="basket")] IAsyncCollector<SignalRMessage> signalRMessage,
            ILogger logger){
                var message = await req.ReadAsStringAsync();
                
                if(string.IsNullOrEmpty(message)){
                          await signalRMessage.AddAsync(
                                    new SignalRMessage {
                                            Target = "get_basket",
                                            Arguments = new[] { JsonConvert.SerializeObject(null) }
                                    });
                    return new NoContentResult();

                }

                var model = JsonConvert.DeserializeObject<Models.Basket>(message); //TODO: To implement queue and database logic
                
               _basket.IsActive = model.IsActive;
               _basket.Items = model.Items;
               _basket.UpdateTotal();
               _basket.Update = DateTime.Now;

               await signalRMessage.AddAsync(
                   new SignalRMessage {
                       Target = "get_basket",
                       Arguments = new[] { JsonConvert.SerializeObject(_basket) }
                   }
               );

               return new NoContentResult();
            }
    }
}
