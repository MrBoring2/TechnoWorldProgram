using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class ElectronicsToDelivery : BaseEntity
    {
        public int ElectronicsId { get; set; }
        public int DeliveryId { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public virtual Delivery Delivery { get; set; }

        public virtual Electronic Electronics { get; set; }
    }
}
