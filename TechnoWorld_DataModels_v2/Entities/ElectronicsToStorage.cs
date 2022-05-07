using Newtonsoft.Json;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class ElectronicsToStorage : BaseEntity
    {
        public int ElectronicsId { get; set; }
        public int StorageId { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public virtual Electronic Electronics { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
