using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Hydra.Basket.Function
{
    public class NegotiateFunc
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, 
            [SignalRConnectionInfo (HubName="basket")] SignalRConnectionInfo connectionInfo) => connectionInfo;
    }
}