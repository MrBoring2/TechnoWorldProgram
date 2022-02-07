
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.Threading.Tasks;

namespace TechoWorld_DataModels
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        [JsonIgnore]
        public virtual ICollection<ElectrnicsType> ElectrnicsTypes { get; set; }
    }
}
