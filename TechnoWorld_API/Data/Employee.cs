using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class Employee
    {
        public Employee()
        {
            WarantyServiceHistories = new HashSet<WarantyServiceHistory>();
        }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public int PostId { get; set; }

        public virtual Post Post { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<WarantyServiceHistory> WarantyServiceHistories { get; set; }
    }
}
