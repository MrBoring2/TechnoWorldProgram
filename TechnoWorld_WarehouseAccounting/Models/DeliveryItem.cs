using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.Models
{
    public class DeliveryItem : INotifyPropertyChanged
    {
        private int count;
        private int id;
        public DeliveryItem(Electronic electronic, int count)
        {

            Electronic = electronic;
            Count = count;
        }
        public int Id { get => id; set { id = value; OnPropertyChanged(); } }
        public Electronic Electronic { get; set; }
        public int Count { get => count; set { count = value >= 0 ? value : 0; OnPropertyChanged(); OnPropertyChanged(nameof(TotalPrice)); OnPropertyChanged(nameof(TotalPriceWithNDS)); } }
        public decimal TotalPrice => Electronic.PurchasePrice * Count;
        public decimal TotalPriceWithNDS => TotalPrice + (TotalPrice * 18 / 100);

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
