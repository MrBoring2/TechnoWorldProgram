using BNS_API.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using TechnoWorld_API.Helpers;
using TechnoWorld_API.Models;
using TechnoWorld_API.Services;
using TechoWorld_DataModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IHubContext<TechnoWorldHub> _hubContext;
        private readonly TechnoWorldContext _context;
        public AuthenticateController(TechnoWorldContext context, IHubContext<TechnoWorldHub> hubContext)
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


                var identity = GetIdentity(model);

                if (identity == null)
                {
                    return BadRequest("Неверное имя пользователя или пароль");
                }

                var token = CreateToken(identity.Result);

                return Ok(token);
            }
            else return BadRequest();
        }

        [HttpPost("/userToken")]
        public async Task<IActionResult> Token(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                {
                    return BadRequest("Поля не должны быть пустыми!");
                }

                if (TechnoWorldHub.ConnectedUsers.TryGetValue(model.UserName, out string connectionId))
                {
                    if (connectionId != null)
                    {
                        Log.Warning($"Провальная попытка входа пользователя {model.UserName}: Данный пользователь уже авторизирован");
                        return BadRequest("Пользователь уже авторизирован!");
                    }
                }
                var identity = GetIdentity(model);

                if (identity.Result == null)
                {
                    Log.Warning($"Провальная попытка входа пользователя {model.UserName}: Неверный логин или пароль");
                    return BadRequest("Неверное имя пользователя или пароль");
                }
                var a = identity.Result.FindFirst("role_id").Value;
                if (model.Programm == "cash")
                {
                    if (identity.Result.FindFirst("role_id").Value != "1")
                    {
                        return BadRequest("Данный пользователь не может здесь авторизироваться");
                    }
                }


                var token = CreateToken(identity.Result);
                Log.Information($"Пользователь {model.UserName} авторизирован в системе");
                return Ok(token);
            }
            else return BadRequest();
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
                full_name = identity.FindFirst("full_name")?.Value.ToString(),
                role_name = identity.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value.ToString(),
                user_id = identity.FindFirst("user_id")?.Value.ToString(),
                role_id = identity.FindFirst("role_id")?.Value.ToString()
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
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "terminalUser")
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }
        private async Task<ClaimsIdentity> GetIdentity(UserLoginModel model)
        {
            Employee user = await _context.Employees.Include(r => r.Role)
                .FirstOrDefaultAsync(x => x.Login == model.UserName && x.Password == model.Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("user_id", user.EmployeeId.ToString()),
                    new Claim("full_name", user.FullName.ToString()),
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                    new Claim("role_id", user.Role.RoleId.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name.ToString())
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }

            return null;
        }


    }
}
