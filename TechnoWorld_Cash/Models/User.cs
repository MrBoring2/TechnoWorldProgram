using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Cash.Models
{
    public class User
    {
        public string Name { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public string Post { get; set; }
        public string FullName { get; set; }
    }
}
