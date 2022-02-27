using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
{
    public partial class Order : BaseEntity
    {
        public Order()
        {
            OrderElectronics = new HashSet<OrderElectronic>();
        }

        public int OrderId { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public string Status { get; set; }
        public virtual ICollection<OrderElectronic> OrderElectronics { get; set; }
    }
}
