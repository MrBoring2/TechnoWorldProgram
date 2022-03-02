using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Cash.Models
{
    public class OrderItem
    {
        public OrderItem(int number, string name, int count, decimal priceForOne, decimal totalPrice)
        {
            Number = number;
            Name = name;
            Count = count;
            PriceForOne = priceForOne;
            TotalPrice = totalPrice;
        }

        public int Number { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal PriceForOne { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
