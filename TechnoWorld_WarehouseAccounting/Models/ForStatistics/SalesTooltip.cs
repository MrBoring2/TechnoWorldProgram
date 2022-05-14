using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_WarehouseAccounting.Models.ForStatistics
{
    public class SalesTooltip
    {
        public SalesTooltip(string name, int count, decimal sales, long ticks)
        {
            Name = name;
            Count = count;
            Sales = sales;
            Ticks = ticks;
        }

        public string Name  { get; set; }
        public int Count { get; set; }
        public decimal Sales { get; set; }
        public long Ticks { get; set; }
    }
}
