using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.POCO_Models
{
    public class ElectronicsToStorage
    {
        [JsonPropertyName("electronicsId")]
        public int ElectronicsId { get; set; }
        [JsonPropertyName("storageId")]
        public int StorageId { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
