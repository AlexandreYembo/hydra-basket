using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Hydra.Basket.Function.Authentication
{
    public static class JwtToken
    {
        public static IEnumerable<Claim> GetClaim(string accessToken) {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(accessToken)?.Claims;
            
        }
    }
}