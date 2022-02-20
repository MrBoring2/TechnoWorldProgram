using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
{
    public partial class Employee : BaseEntity
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
