using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class Delivery : BaseEntity
    {
        public Delivery()
        {
            ElectronicsToDeliveries = new HashSet<ElectronicsToDelivery>();
        }

        public int DelivertId { get; set; }
        public string DeliveryNumber { get; set; }
        public int StorageId { get; set; }
        public int SupplierId { get; set; }
        public DateTime DateOfOrder { get; set; }
        public DateTime DateOfDelivery { get; set; }
        public int EmployeeId { get; set; }
        public int StatusId { get; set; }
        [NotMapped]
        public decimal TotalPrice => ElectronicsToDeliveries.Any(p => p.Electronics == null) ? 0 : ElectronicsToDeliveries.Sum(p => p.Electronics.PurchasePrice * p.Quantity + (p.Electronics.PurchasePrice * p.Quantity * 18 / 100));
        [NotMapped]
        public decimal TotalCount => ElectronicsToDeliveries.Sum(p => p.Quantity);
        public virtual Employee Employee { get; set; }
        public virtual Storage Storage { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Status Status { get; set; }
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
    }
}
