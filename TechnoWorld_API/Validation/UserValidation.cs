using System.Security.Claims;
using TechnoWorld_API.Models;
using TechnoWorld_API.Models.ApiResponse;
using TechnoWorld_API.Models.Auth;
using TechnoWorld_API.Services;

namespace TechnoWorld_API.Validation
{
    public class UserValidation
    {
        public static ValidationResult ValidateLoginPassword(UserLoginModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                return new ValidationResult(false, "Поля не должны быть пустыми!");
            }

            if (TechnoWorldHub.ConnectedUsers.TryGetValue(model.UserName, out string connectionId))
            {
                if (connectionId != null)
                {

                    LogService.LodMessage($"Провальная попытка входа пользователя {model.UserName}: Данный пользователь уже авторизирован", LogLevel.Warning);
                    return new ValidationResult(false, "Пользователь уже авторизирован!");
                }
            }

            return new ValidationResult(true, "Success");
        }
        public static ValidationResult ValudateProgramm(UserLoginModel model, ClaimsIdentity userClaims)
        {
            if ((model.Programm == "cash" && userClaims.FindFirst("role_id").Value != "1") ||
                (model.Programm == "warehouse_accounting" && userClaims.FindFirst("role_id").Value == "1"))
            {
                return new ValidationResult(false, "Данный пользователь не может здесь авторизироваться");
            }
            return new ValidationResult(true, "Success");
        }
    }
}
