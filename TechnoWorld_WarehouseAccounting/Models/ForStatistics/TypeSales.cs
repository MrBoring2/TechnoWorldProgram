using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_WarehouseAccounting.Models.ForStatistics
{
    public class TypeSales
    {
        public TypeSales(ElectrnicsType electrnicsType, int count, decimal sales)
        {
            ElectrnicsType = electrnicsType;
            Count = count;
            Sales = sales;
        }

        public ElectrnicsType ElectrnicsType { get; set; }
        public string Name => ElectrnicsType.Name;
        public int Count { get; set; }
        public decimal Sales { get; set; }
    }
}
