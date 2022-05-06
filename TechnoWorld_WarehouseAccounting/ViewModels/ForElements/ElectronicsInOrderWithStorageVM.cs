using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Common;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_WarehouseAccounting.ViewModels.ForElements
{
    public class ElectronicsInOrderWithStorageVM : NotifyPropertyChangedBase
    {
        private Storage storage;

        public ElectronicsInOrderWithStorageVM(Storage storage, Electronic electronic, int count)
        {
            Storage = storage;
            Electronic = electronic;
            Count = count;
        }

        public Electronic Electronic { get; set; }
        public Storage Storage { get => storage; set { storage = value; OnPropertyChanged(); } }
        public int Count { get; set; }
        public decimal TotalPrice => Electronic.SalePrice * Count;
        public string IsOfferedForSale => Electronic.IsOfferedForSale ? "Выставлен на продажу" : "Снят с продажи";
        public int CountInStorage => Storage.ElectronicsToStorages.Count > 0 ? Storage.ElectronicsToStorages.FirstOrDefault(p => p.ElectronicsId == Electronic.ElectronicsId).Quantity : 0;
    }
}
