using System;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class OrderElectronic : BaseEntity
    {
        public int ElectronicsId { get; set; }
        public int OrderId { get; set; }
        public int Count { get; set; }

        public virtual Electronic Electronics { get; set; }
        public virtual Order Order { get; set; }
    }
}
