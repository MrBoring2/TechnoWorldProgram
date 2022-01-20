using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class Storage
    {
        public Storage()
        {
            Deliveries = new HashSet<Delivery>();
            ElectronicsToStorages = new HashSet<ElectronicsToStorage>();
        }

        public int StorageId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
        public virtual ICollection<ElectronicsToStorage> ElectronicsToStorages { get; set; }
    }
}
