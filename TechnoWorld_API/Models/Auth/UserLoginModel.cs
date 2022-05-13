using TechnoWorld_API.Models.Abstractions;

namespace TechnoWorld_API.Models.Auth
{
    public class UserLoginModel : BaseLoginModel
    {
        public string UserName { get; set; }
        public string HashPass { get; set; }
        public string Programm { get; set; }
    }
}
