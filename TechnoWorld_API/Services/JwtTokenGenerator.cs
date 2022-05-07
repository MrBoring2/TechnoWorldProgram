using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TechnoWorld_API.Helpers;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_API.Services
{
    public class JwtTokenGenerator
    {
        public static JwtTokenResult GenerateToken(ClaimsIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromHours(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var result = new JwtTokenResult
            {
                access_token = encodedJwt,
                user_name = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value.ToString(),
                role_name = identity.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value.ToString(),
            };

            return result;
        }
    }
}
