using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
{
    public partial class OrderElectronic : BaseEntity
    {
        public int ElectronicsId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

        public virtual Electronic Electronics { get; set; }
        public virtual Order Order { get; set; }
    }
}
