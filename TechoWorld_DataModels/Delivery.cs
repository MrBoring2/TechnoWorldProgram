﻿using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
{
    public partial class Delivery : BaseEntity
    {
        public Delivery()
        {
            ElectronicsToDeliveries = new HashSet<ElectronicsToDelivery>();
        }

        public int DelivertId { get; set; }
        public int StorageId { get; set; }
        public int SupplierId { get; set; }
        public DateTime DateOfDelivery { get; set; }
        public int EmployeeId { get; set; }
        public int StatusId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Storage Storage { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
    }
}
