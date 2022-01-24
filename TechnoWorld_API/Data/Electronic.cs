using System;
using System.Collections.Generic;
using TechnoWorld_API.Data;

#nullable disable

namespace BNS_API.Data
{
    public partial class Electronic
    {
        public Electronic()
        {
            ElectronicsToDeliveries = new HashSet<ElectronicsToDelivery>();
            ElectronicsToStorages = new HashSet<ElectronicsToStorage>();
            OrderElectronics = new HashSet<OrderElectronic>();
            WarantyServiceHistories = new HashSet<WarantyServiceHistory>();
        }

        public int ElectronicsId { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int ManufactrurerId { get; set; }
        public int TypeId { get; set; }
        public byte[] Image { get; set; }
        public int HarantyMonth { get; set; }
        public string ManufacturerСountry { get; set; }
        public string Color { get; set; }
        public double Weight { get; set; }
        public virtual Manufacturer Manufactrurer { get; set; }
        public virtual ElectrnicsType Type { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }
        public virtual ICollection<OrderElectronic> OrderElectronics { get; set; }
        public virtual ICollection<WarantyServiceHistory> WarantyServiceHistories { get; set; }
    }
}
