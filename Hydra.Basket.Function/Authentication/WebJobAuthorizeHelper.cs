using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Azure.WebJobs.Description;

namespace Hydra.Basket.Function.Authentication
{
    /// <summary>
    /// TODO: Implement the best way for authentication process.
    /// </summary>
    /// 
    public interface IWebJobAuthorizeHelper
    {
        string GetUserId(HttpRequest req);
    }
    public class WebJobAuthorizeHelper : IWebJobAuthorizeHelper
    {
        private const string AUTH_HEADER_NAME = "Authorization";
        private const string BEARER_PREFIX = "Bearer ";
        public string GetUserId(HttpRequest req)
        {
            if (req.Headers.ContainsKey(AUTH_HEADER_NAME) && 
               req.Headers[AUTH_HEADER_NAME].ToString().StartsWith(BEARER_PREFIX))
            {
                   string token = req.Headers["Authorization"].ToString().Substring(BEARER_PREFIX.Length);
                   return JwtToken.GetUserId(token);
            }
            else
                throw new UnauthorizedAccessException("Unathorized Access");

        }
    }
}