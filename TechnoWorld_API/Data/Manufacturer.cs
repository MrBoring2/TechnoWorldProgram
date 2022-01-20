using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace BNS_API.Data
{
    public partial class Manufacturer
    {
        public Manufacturer()
        {
            Electronics = new HashSet<Electronic>();
        }

        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Electronic> Electronics { get; set; }
    }
}
