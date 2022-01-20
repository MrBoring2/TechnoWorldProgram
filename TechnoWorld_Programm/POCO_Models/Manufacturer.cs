using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.POCO_Models
{
    public class Manufacturer
    {
        [JsonPropertyName("manufacturerId")]
        public int ManufacturerId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}
