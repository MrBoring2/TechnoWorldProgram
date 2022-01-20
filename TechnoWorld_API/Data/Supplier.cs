using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class Supplier
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
