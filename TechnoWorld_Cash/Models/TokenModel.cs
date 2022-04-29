using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Terminal.Models
{
    public class TokenModel
    {
        public string access_token { get; set; }
        public string full_name { get; set; }
        public string user_name{ get; set; }
        public string role_name { get; set; }
        public string post { get; set; }
        public int user_id { get; set; }
        public int role_id { get; set; }
    }
}
