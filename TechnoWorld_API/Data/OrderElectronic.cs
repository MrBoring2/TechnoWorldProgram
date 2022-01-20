using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class OrderElectronic
    {
        public int ElectronicsId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

        public virtual Electronic Electronics { get; set; }
        public virtual Order Order { get; set; }
    }
}
