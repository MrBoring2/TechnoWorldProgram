using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace BNS_API.Data
{
    public partial class ElectrnicsType
    {
        public ElectrnicsType()
        {
            Electronics = new HashSet<Electronic>();
        }

        public int TypeId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]

        public virtual ICollection<Electronic> Electronics { get; set; }
    }
}
