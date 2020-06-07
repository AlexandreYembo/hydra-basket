using System.Linq;
using Hydra.Basket.Function.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace Hydra.Basket.Function
{
    public class NegotiateFunc
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
       
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req, 
            IBinder binder,
            ILogger log){
            if (req.Headers.ContainsKey(AUTH_HEADER_NAME) && req.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX))
            {
                string token = req.Headers["Authorization"].ToString().Substring(BEARER_PREFIX.Length);

                var jwtClaim = JwtToken.GetClaim(token);
                string userId = jwtClaim.Where(w => w.Type == "sub").FirstOrDefault().Value;
                var connectionInfo = binder.Bind<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute{HubName = "basket", UserId = userId});
                return connectionInfo;
            }
            log.LogError("Cannot connect to the Hub. Invalid access Token");
            return null;
        }
    }
}