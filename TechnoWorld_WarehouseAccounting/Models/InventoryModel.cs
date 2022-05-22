using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_WarehouseAccounting.Models
{
    public class InventoryModel : NotifyPropertyChangedBase
    {
        private int id;
        private Electronic electronics;
        private int factAmount;
        private int buhAmount;
        private decimal price;

        public InventoryModel(Electronic _electronic, int _buhAmount, decimal _price)
        {
            Electronics = _electronic;
            BuhAmount = _buhAmount;
            Price = _price;
            OnPropertyChanged(nameof(Deviation));
        }

        public Electronic Electronics { get => electronics; private set { electronics = value; OnPropertyChanged(); } }
        public int Id { get => id; set { id = value; OnPropertyChanged(); } }
        public int FactAmount { get => factAmount; set { factAmount = value < 0 ? factAmount : value; OnPropertyChanged(); OnPropertyChanged(nameof(FactTotalPrice)); OnPropertyChanged(nameof(Deviation)); } }
        public int BuhAmount { get => buhAmount; set { buhAmount = value; OnPropertyChanged(); OnPropertyChanged(nameof(BuhTotalPrice)); OnPropertyChanged(nameof(Deviation)); } }
        public decimal Price { get => price; private set { price = value; OnPropertyChanged(); } }
        public decimal FactTotalPrice => FactAmount == 0 ? 0 : Price * FactAmount;
        public decimal BuhTotalPrice => BuhAmount == 0 ? 0 : Price * BuhAmount;
        public decimal Deviation => FactAmount - BuhAmount;
    }
}
