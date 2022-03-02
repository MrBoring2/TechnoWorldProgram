using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace TechoWorld_DataModels
{
    public partial class Electronic : BaseEntity
    {
        public Electronic()
        {
            ElectronicsToDeliveries = new HashSet<ElectronicsToDelivery>();
            ElectronicsToStorages = new HashSet<ElectronicsToStorage>();
            OrderElectronics = new HashSet<OrderElectronic>();
        }

        public int ElectronicsId { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int ManufactrurerId { get; set; }
        public int TypeId { get; set; }
        public byte[] Image { get; set; }
        public string ManufacturerСountry { get; set; }
        public string Color { get; set; }
        public double Weight { get; set; }
        public virtual Manufacturer Manufacturer { get; set; }
        public virtual ElectrnicsType Type { get; set; }
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }
        public virtual ICollection<OrderElectronic> OrderElectronics { get; set; }
        [NotMapped]
        public string ManufacturerName
        {
            get => Manufacturer?.Name;
        }
        [NotMapped]
        public int AmountInStorage
        {
            get
            {
                return ElectronicsToStorages.Count() > 0 && ElectronicsToStorages != null ? ElectronicsToStorages.FirstOrDefault().Quantity : 0;
            }
        }

        public object GetProperty(string property)
        {
            return GetType().GetProperty(property).GetValue(this);
        }
    }
}
