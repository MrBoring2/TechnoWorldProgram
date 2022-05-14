﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_WarehouseAccounting.Models.ForStatistics
{
    public class SaleModel
    {
        public SaleModel(string name, int count, decimal sales)
        {
            Name = name;
            Count = count;
            TotalSales = sales;
        }

        public string Name { get; set; }
        public int Count { get; set; }
        public decimal TotalSales { get; set; }
    }
}
