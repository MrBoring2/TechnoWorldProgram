using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
{
    public partial class Delivery
    {
        public Delivery()
        {
            ElectronicsToDeliveries = new HashSet<ElectronicsToDelivery>();
        }

        public int DelivertId { get; set; }
        public int StorageId { get; set; }
        public int SupplierId { get; set; }
        public DateTime DateOfDelivery { get; set; }

        public virtual Storage Storage { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
    }
}
