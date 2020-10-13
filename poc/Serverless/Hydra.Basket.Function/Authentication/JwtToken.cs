using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Hydra.Basket.Function.Authentication
{
    public static class JwtToken
    {
        public static IEnumerable<Claim> GetClaim(string accessToken) {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(accessToken)?.Claims;
        }

        public static string GetUserId(string accessToken){
            IEnumerable<Claim> claims = GetClaim(accessToken);
            return claims?.Where(w => w.Type == "sub").FirstOrDefault().Value;
        }
    }
}