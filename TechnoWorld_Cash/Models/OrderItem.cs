using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;

namespace TechnoWorld_Cash.Models
{
    public class OrderItem
    {
        public OrderItem(int number, Electronic electronic, int count)
        {
            Number = number;
            Electronic = electronic;
            Count = count;
        }

        public int Number { get; set; }
        public Electronic Electronic { get; set; }
        public int Count { get; set; }
        public decimal PriceForOne => Electronic.SalePrice;
        public decimal TotalPrice => PriceForOne * Count;
    }
}
