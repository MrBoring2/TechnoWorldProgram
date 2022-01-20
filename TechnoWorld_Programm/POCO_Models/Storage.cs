using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.POCO_Models
{
    public class Storage
    {
        [JsonPropertyName("storageId")]
        public int StorageId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("deliveries")]
        public virtual ICollection<Delivery> Deliveries { get; set; }
        [JsonPropertyName("electronicsToStorages")]
        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }
    }
}
