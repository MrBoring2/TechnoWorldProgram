using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace TechoWorld_DataModels_v2
{
    public partial class Storage : BaseEntity
    {
        public Storage()
        {
            Deliveries = new HashSet<Delivery>();
            ElectronicsToStorages = new HashSet<ElectronicsToStorage>();
        }

        public int StorageId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Delivery> Deliveries { get; set; }
     
        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }
    }
}
