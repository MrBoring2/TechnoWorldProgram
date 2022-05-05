
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TechoWorld_DataModels_v2
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
