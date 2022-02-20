using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
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
