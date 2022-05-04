﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechoWorld_DataModels
{
    public partial class Order : BaseEntity
    {
        public Order()
        {
            OrderElectronics = new HashSet<OrderElectronic>();
        }

        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public int? EmployeeId { get; set; }

        public virtual Employee Employee { get; set; }
        public int StatusId { get; set; }
        public virtual Status Status { get; set; }

        public int ProductCount => OrderElectronics.Count;
        public decimal OrderPrice => OrderElectronics.Sum(p => p.Electronics.SalePrice * p.Count);

        public virtual ICollection<OrderElectronic> OrderElectronics { get; set; }
    }
}
