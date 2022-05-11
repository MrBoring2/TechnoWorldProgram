using TechnoWorld_API.Data;
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
using TechoWorld_DataModels_v2;
using TechnoWorld_API.Extentions;
using TechnoWorld_API.Validation;
using Microsoft.AspNetCore.Identity;
using TechoWorld_DataModels_v2.Entities;
using TechoWorld_DataModels_v2.Models;
using TechnoWorld_API.Models.Auth;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TechnoWorld_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly TechnoWorldContext _context;
        public AuthenticateController(TechnoWorldContext context)
        {
            _context = context;
        }

        [HttpPost("/terminalToken")]
        public IActionResult Token(TerminalLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var terminalClaims = ClaimsExtentions.BuildClaimsForTerminal(model);

                var token = JwtTokenGenerator.GenerateToken(terminalClaims);

                if (token == null)
                {
                    return BadRequest("Произошла ошибка при авторизации!");
                }
                HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token.access_token);

                return Ok(new AuthResponseModel
                {
                    user_name = terminalClaims.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value.ToString(),
                    full_name = "terminal",
                    role_name = "terminal",
                    user_id = 0,
                    role_id = 0,
                    post = "none"
                });
            }
            else return BadRequest();
        }

        [HttpPost("/userToken")]
        public async Task<IActionResult> Token(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                ClaimsIdentity userClaims = null;
                var validationLoginPasswordResult = UserValidation.ValidateLoginPassword(model);
               
                if (validationLoginPasswordResult.Result == true)
                {
                    var user = await TryGetUser(model.UserName, model.Password);
                    if (user == null)
                    {
                        LogService.LodMessage($"Провальная попытка входа пользователя {model.UserName}: Неверный логин или пароль", LogLevel.Warning);
                        return BadRequest("Неверное имя пользователя или пароль");
                    }
                    else
                    {
                        userClaims = ClaimsExtentions.BuildClaimsForUser(user);
                        var validationProdgrammResult = UserValidation.ValudateProgramm(model, userClaims);
                        if(validationProdgrammResult.Result == false)
                        {
                            return BadRequest(validationProdgrammResult.Message);
                        }
                    }
                }
                else
                {
                    return BadRequest(validationLoginPasswordResult.Message);
                }
                var token = JwtTokenGenerator.GenerateToken(userClaims);

                if (token == null)
                {
                    return BadRequest("Произошла ошибка при авторизации!");
                }

                LogService.LodMessage($"Пользователь {model.UserName} авторизирован в системе", LogLevel.Info);

                HttpContext.Response.Cookies.Append(".AspNetCore.Application.Id", token.access_token);

                return Ok(new AuthResponseModel
                {
                    user_name = userClaims.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value.ToString(),
                    full_name = userClaims.FindFirst("full_name")?.Value.ToString(),
                    role_name = userClaims.FindFirst(ClaimsIdentity.DefaultRoleClaimType).Value.ToString(),
                    user_id = Convert.ToInt32(userClaims.FindFirst("user_id")?.Value),
                    role_id = Convert.ToInt32(userClaims.FindFirst("role_id")?.Value),
                    post = userClaims.FindFirst("post")?.Value.ToString()
                });
            }
            else return BadRequest("Модель не валидна");
        }

        private async Task<Employee> TryGetUser(string login, string password)
        {
            return await _context.Employees.Include(p => p.Role).Include(p => p.Post).FirstOrDefaultAsync(p => p.Login == login && p.Password == password);
        }

    }
}
