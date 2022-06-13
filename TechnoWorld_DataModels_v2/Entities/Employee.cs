using System;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class Employee : BaseEntity, ICloneable
    {
        public Employee()
        {
        }
        public Employee(int empoloyeeId, string fullName, string phoneNumber, DateTime dateOfBirth, int roleId, int postId,
                        string login, string password, string email, string passport, string gender, Post post, Role role,
                        ICollection<Delivery> deliveries, ICollection<Order> orders)
        {
            EmployeeId = empoloyeeId;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            RoleId = roleId;
            PostId = postId;
            Login = login;
            Password = password;
            Email = email;
            Passport = passport;
            Gender = gender;
            Post = post;
            Role = role;
            Deliveries = deliveries;
            Orders = orders;
        }
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public int PostId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Passport { get; set; }
        public string Gender { get; set; }
        public virtual Post Post { get; set; }
        public virtual Role Role { get; set; }
        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public object Clone()
        {
            return new Employee(EmployeeId, FullName, PhoneNumber, DateOfBirth, RoleId, PostId,
                                Login, Password, Email, Passport, Gender, Post, Role,
                                Deliveries, Orders);
        }
    }
}
