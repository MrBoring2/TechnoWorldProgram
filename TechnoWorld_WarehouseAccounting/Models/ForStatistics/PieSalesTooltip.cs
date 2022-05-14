using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_WarehouseAccounting.Models.ForStatistics
{
    public class PieSalesTooltip
    {
        public PieSalesTooltip(string name, int count, decimal sales)
        {
            Name = name;
            Count = count;
            Sales = sales;
        }

        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Sales { get; set; }

    }
}
