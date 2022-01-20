using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class ElectronicsToDelivery
    {
        public int ElectronicsId { get; set; }
        public int DeliveryId { get; set; }
        public int Quantity { get; set; }

        public virtual Delivery Delivery { get; set; }
        public virtual Electronic Electronics { get; set; }
    }
}
