using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TechnoWorld_API.Data;

#nullable disable

namespace BNS_API.Data
{
    public partial class ElectrnicsType
    {
        public ElectrnicsType()
        {
           // Categories = new HashSet<Category>();
        }

        public int TypeId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
