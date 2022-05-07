
using Newtonsoft.Json;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        [JsonIgnore]
        public virtual ICollection<ElectrnicsType> ElectrnicsTypes { get; set; }
    }
}
