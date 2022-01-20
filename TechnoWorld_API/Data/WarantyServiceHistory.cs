using System;
using System.Collections.Generic;

#nullable disable

namespace BNS_API.Data
{
    public partial class WarantyServiceHistory
    {
        public int WarantyServiceId { get; set; }
        public int EmployeeId { get; set; }
        public int ElectronicsId { get; set; }
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Problem { get; set; }

        public virtual Electronic Electronics { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Order Order { get; set; }
    }
}
