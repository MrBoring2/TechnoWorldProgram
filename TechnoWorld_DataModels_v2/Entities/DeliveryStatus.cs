using System;
using System.Collections.Generic;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class DeliveryStatus
    {
        public DeliveryStatus()
        {
            Deliveries = new HashSet<Delivery>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Delivery> Deliveries { get; set; }
    }
}
