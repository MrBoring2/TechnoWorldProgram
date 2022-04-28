using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class DeliveryManagementPageVM : BasePageVM
    {
        public DeliveryManagementPageVM()
        {
            OpenDeliveryWindowCommand = new RelayCommand(OpenDeliveryWindow);
        }
        public RelayCommand OpenDeliveryWindowCommand { get; set; }
        private async void OpenDeliveryWindow(object obj)
        {
            var deliveryWindowVM = new DeliveryWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(deliveryWindowVM));

            if (deliveryWindowVM.DialogResult == true)
            {
                //await LoadElectronics();
                CustomMessageBox.Show($"Заказ поиставщику номер упешно добавлен", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
