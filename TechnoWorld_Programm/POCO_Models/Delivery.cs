using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.POCO_Models
{
    public class Delivery
    {
        [JsonPropertyName("delivertId")]
        public int DelivertId { get; set; }
        [JsonPropertyName("storageId")]
        public int StorageId { get; set; }
        [JsonPropertyName("supplierId")]
        public int SupplierId { get; set; }
        [JsonPropertyName("dateOfDelivery")]
        public DateTime DateOfDelivery { get; set; }
        [JsonPropertyName("storage")]
        public virtual Storage Storage { get; set; }
        [JsonPropertyName("supplier")]
        public virtual Supplier Supplier { get; set; }
        [JsonPropertyName("electronicsToDeliveries")]
        public virtual ICollection<ElectronicsToDelivery> ElectronicsToDeliveries { get; set; }
    }
}
