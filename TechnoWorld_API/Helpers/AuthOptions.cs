using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TechnoWorld_API.Helpers
{
    public class AuthOptions
    {
        public const string ISSUER = "TechnoWorldServer";
        public const string AUDIENCE = "TechnoWorldClient";
        private const string KEY = "texchnoworldsecretkey";
        public const int LIFETIME = 3;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
