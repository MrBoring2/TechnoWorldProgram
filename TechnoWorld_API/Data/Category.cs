using BNS_API.Data;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TechnoWorld_API.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        [JsonIgnore]
        public virtual ICollection<ElectrnicsType> ElectrnicsTypes { get; set; }
    }
}
