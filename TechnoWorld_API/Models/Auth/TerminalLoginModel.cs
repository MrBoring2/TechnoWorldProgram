using TechnoWorld_API.Models.Abstractions;

namespace TechnoWorld_API.Models.Auth
{
    public class TerminalLoginModel : BaseLoginModel
    {
        public string TerminalName { get; set; }
    }
}
