using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class Order
    {
        public Order()
        {
            OrderElectronics = new HashSet<OrderElectronic>();
            WarantyServiceHistories = new HashSet<WarantyServiceHistory>();
        }

        public int OrderId { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public string Status { get; set; }
        public string DeliveryAddress { get; set; }
        public int ClientId { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<OrderElectronic> OrderElectronics { get; set; }
        public virtual ICollection<WarantyServiceHistory> WarantyServiceHistories { get; set; }
    }
}
