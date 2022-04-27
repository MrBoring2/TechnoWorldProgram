using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace TechoWorld_DataModels
{
    public partial class ElectronicsToStorage  : BaseEntity
    {
        public int ElectronicsId { get; set; }
        public int StorageId { get; set; }
        public int Quantity { get; set; }
        [JsonIgnore]
        public virtual Electronic Electronics { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
