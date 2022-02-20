using BNS_API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using TechnoWorld_API.Helpers;
using TechnoWorld_API.Models;
using TechnoWorld_API.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly BNSContext _context;
        public AuthenticateController(BNSContext context, IHubContext<TechnoWorldHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        // GET: api/<AuthenticateController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [HttpPost("/terminalToken")]
        public async Task<IActionResult> Token(TerminalLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var identity = await GetIdentity(model);

                if (identity == null)
                {
                    return BadRequest("Неверное имя пользователя или пароль");
                }

                var token = CreateToken(identity);

                return Ok(token);
            }
            else return BadRequest();
        }

        [HttpPost("/userToken")]
        public async Task<IActionResult> Token(UserLoginModel model)
        {
            //var identity = await GetIdentity(login, password);

            //if (identity == null)
            //{
            //    return BadRequest("Неверное имя пользователя или пароль");
            //}

            //var token = CreateToken(identity);

            //return Ok(response);
            return BadRequest();
        }
        private dynamic CreateToken(ClaimsIdentity identity)
        {
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

            //HttpContext.Session.SetString("JWToken", encodedJwt);
            var response = new
            {
                access_token = encodedJwt,
                user_name = identity.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value.ToString(),
                role_name = identity.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value.ToString()
            };

            return response;
        }
        private async Task<ClaimsIdentity> GetIdentity(TerminalLoginModel model)
        {


            if (model != null)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, model.TerminalName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, model.RoleName.ToString())
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                //await AddUserToGroup((Roles)Convert.ToInt32(claimsIdentity.FindFirst("role_id").Value), claimsIdentity);

                return claimsIdentity;
            }

            return null;
        }
        private async Task<ClaimsIdentity> GetIdentity(UserLoginModel model)
        {
            //User user = await _context.Users.Include(r => r.Role)
            //    .FirstOrDefaultAsync(x => x.Login == login && x.Password == password);

            //bool atempt;
            //if (user is null)
            //    atempt = false;
            //else atempt = true;

            //LogUser(login, atempt);


            //if (user != null)
            //{
            //    user.LastEnterDate = DateTime.Now;
            //    _context.Entry(user).State = EntityState.Modified;
            //    _context.SaveChanges();

            //    var claims = new List<Claim>
            //    {
            //        new Claim("user_login", user.Login),
            //        new Claim("user_id", user.Id.ToString()),
            //        new Claim("user_name_surname", user.FullName),
            //        new Claim("role_id", user.Role.Id.ToString()),
            //        new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.RoleName.ToString())
            //    };
            //    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            //    //await AddUserToGroup((Roles)Convert.ToInt32(claimsIdentity.FindFirst("role_id").Value), claimsIdentity);

            //    return claimsIdentity;
            //}

            return null;
        }


    }
}
