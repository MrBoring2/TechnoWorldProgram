using System;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class Supplier : BaseEntity
    {
        public Supplier()
        {
            Deliveries = new HashSet<Delivery>();
        }

        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
    }
}
