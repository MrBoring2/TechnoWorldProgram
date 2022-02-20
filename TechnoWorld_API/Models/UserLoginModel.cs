namespace TechnoWorld_API.Models
{
    public class UserLoginModel : BaseLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
