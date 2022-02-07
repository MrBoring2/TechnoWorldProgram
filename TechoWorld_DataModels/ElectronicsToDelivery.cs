using System;
using System.Collections.Generic;


namespace TechoWorld_DataModels
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
