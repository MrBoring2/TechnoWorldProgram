using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechoWorld_DataModels_v2
{
    public class JwtTokenResult
    {
        public string access_token { get; set; }
        public string user_name { get; set; }
        public string role_name { get; set; }
    }
}
