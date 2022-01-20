using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.POCO_Models
{
    public class ElectronicsToDelivery
    {
        [JsonPropertyName("electronicsId")]
        public int ElectronicsId { get; set; }
        [JsonPropertyName("deliveryId")]
        public int DeliveryId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
