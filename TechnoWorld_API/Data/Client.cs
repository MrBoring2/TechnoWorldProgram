using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class Client
    {
        public Client()
        {
            Orders = new HashSet<Order>();
        }

        public int ClientId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
