using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels_v2
{
    public partial class Employee : BaseEntity
    {
        public Employee()
        {
        }

        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public int PostId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public virtual Post Post { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
