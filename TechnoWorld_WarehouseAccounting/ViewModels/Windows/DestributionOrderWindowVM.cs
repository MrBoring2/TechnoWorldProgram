using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.ForElements;
using TechoWorld_DataModels_v2;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using System.Windows;
using WPF_VM_Abstractions;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using TechoWorld_DataModels_v2.Entities;
using MaterialNotificationLibrary.Enums;
using MaterialNotificationLibrary;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class DestributionOrderWindowVM : BaseModalWindowVM
    {
        private ObservableCollection<Storage> storages;
        private ObservableCollection<ElectronicsInOrderWithStorageVM> products;
        private Storage selectedStorage;

        public DestributionOrderWindowVM(Order order)
        {
            Initialize(order);
            LoadData();
            ApiService.Instance.GetHubConnection.On<string>("UpdateOrders", (deliveries) =>
            {
                LoadData();
            });
        }


        public RelayCommand DestributeOrderCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public Order Order { get; set; }
        public string OrderNumber => $"Заказ № {Order.OrderNumber} от {Order.DateOfRegistration.ToString("d")}";
        public ObservableCollection<Storage> Storages { get => storages; set { storages = value; OnPropertyChanged(); } }
        public Storage SelectedStorage { get => selectedStorage; set { selectedStorage = value; OnPropertyChanged(); LoadProducts(); } }
        public ObservableCollection<ElectronicsInOrderWithStorageVM> Products { get => products; set { products = value; OnPropertyChanged(); } }

        private void Initialize(Order order)
        {
            DestributeOrderCommand = new RelayCommand(DestributeOrder);
            CancelCommand = new RelayCommand(Cancel);
            Order = order;
        }

        private async void LoadData()
        {
            await LoadStorages();
        }
        private void LoadProducts()
        {
            Products = new ObservableCollection<ElectronicsInOrderWithStorageVM>(Order.OrderElectronics.Select(p => new ElectronicsInOrderWithStorageVM(SelectedStorage, p.Electronics, p.Count)));
        }
        private async Task LoadStorages()
        {
            var response = await ApiService.Instance.GetRequest("api/Storages");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Storages = new ObservableCollection<Storage>(JsonConvert.DeserializeObject<List<Storage>>(response.Content));
                SelectedStorage = Storages.FirstOrDefault();

            }
        }
        private async void DestributeOrder(object obj)
        {
            var result = MaterialNotification.Show("Подтверждение", $"Подтвердите выдачу товара.", MaterialNotificationButton.YesNo, MaterialNotificationImage.Question);
            if (result == MaterialNotificationResult.Yes)
            {
                var response = await ApiService.Instance.PutRequest($"api/Orders/Distribute", Order.OrderId, SelectedStorage.StorageId);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    DialogResult = true;
                }
                else
                {
                    MaterialNotification.Show("Ошибка", $"{JsonConvert.DeserializeObject<string>(response.Content)}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                }
            }
        }

        private void Cancel(object obj)
        {
            DialogResult = false;
        }


    }
}
