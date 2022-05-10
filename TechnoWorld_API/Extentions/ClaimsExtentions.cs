using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using TechnoWorld_API.Models;
using TechnoWorld_API.Models.Auth;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_API.Extentions
{
    public static class ClaimsExtentions
    {
        public static ClaimsIdentity BuildClaimsForUser<T>(this T user)
            where T : Employee
        {         
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name),
                new Claim("role_id", user.Role.RoleId.ToString()),             
                new Claim("full_name", user.FullName.ToString()),
                new Claim("user_id", user.EmployeeId.ToString()),
                new Claim("post", user.Post.Name.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.TimeOfDay.Ticks.ToString(),
                    ClaimValueTypes.Integer64)
            };
            //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
        public static ClaimsIdentity BuildClaimsForTerminal<T>(this T terminal)
            where T : TerminalLoginModel
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, terminal.TerminalName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "terminalUser")
                };
            return new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}
