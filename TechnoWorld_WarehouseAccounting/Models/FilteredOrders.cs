using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.Models
{
    public class FilteredOrders
    {
        public FilteredOrders() { }
        public FilteredOrders(List<Order> orders, int totalFilteredCount)
        {
            Orders = orders;
            TotalFilteredCount = totalFilteredCount;
        }

        public List<Order> Orders { get; set; }
        public int TotalFilteredCount { get; set; }
    }
}
