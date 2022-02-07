using System;
using System.Collections.Generic;


namespace TechoWorld_DataModels
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
