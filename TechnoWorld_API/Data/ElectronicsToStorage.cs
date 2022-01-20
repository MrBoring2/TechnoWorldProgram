using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class ElectronicsToStorage
    {
        public int ElectronicsId { get; set; }
        public int StorageId { get; set; }
        public int Quantity { get; set; }

        public virtual Electronic Electronics { get; set; }
        public virtual Storage Storage { get; set; }
    }
}
