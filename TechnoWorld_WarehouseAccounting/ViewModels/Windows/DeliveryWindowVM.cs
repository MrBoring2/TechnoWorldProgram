using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class DeliveryWindowVM : BaseModalWindowVM
    {
        private ObservableCollection<Supplier> suppliers;
        private ObservableCollection<Storage> storages;
        private ObservableCollection<DeliveryItem> deliveryItems;
        private DeliveryItem selectedDeliveryItem;
        private Storage selectedStorage;
        private Supplier selectedSupplier;
        private string deliveryNumber;
        public DeliveryWindowVM()
        {
            Initialize();
            LoadData();

        }


        public RelayCommand AddProductCommand { get; set; }
        public RelayCommand RemoveProductCommand { get; set; }
        public DeliveryItem SelectedDeliveryItem { get => selectedDeliveryItem; set { selectedDeliveryItem = value; OnPropertyChanged(); } }
        public string DeliveryNumber { get => deliveryNumber; set { deliveryNumber = value; OnPropertyChanged(); } }
        public Storage SelectedStorage { get => selectedStorage; set { selectedStorage = value; OnPropertyChanged(); } }
        public Supplier SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; OnPropertyChanged(); } }
        public ObservableCollection<DeliveryItem> DeliveryItems { get => deliveryItems; set { deliveryItems = value; OnPropertyChanged(); } }
        public ObservableCollection<Supplier> Suppliers { get => suppliers; set { suppliers = value; OnPropertyChanged(); } }
        public ObservableCollection<Storage> Storages { get => storages; set { storages = value; OnPropertyChanged(); } }
        private void Initialize()
        {
            DeliveryNumber = GenerateDeliveryNumber();
            AddProductCommand = new RelayCommand(AddProduct);
            RemoveProductCommand = new RelayCommand(RemoveProduct);
            DeliveryItems = new ObservableCollection<DeliveryItem>();
            DeliveryItems.CollectionChanged += DeliveryItems_CollectionChanged;
        }

        private void LoadData()
        {
            LoadStorages();
            LoadSuppliers();
        }


        private async void LoadSuppliers()
        {
            var response = await ApiService.GetRequest("api/Suppliers");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Suppliers = new ObservableCollection<Supplier>(JsonConvert.DeserializeObject<List<Supplier>>(response.Content));
                SelectedSupplier = Suppliers.FirstOrDefault();
            }
        }

        private async void LoadStorages()
        {
            var response = await ApiService.GetRequest("api/Storages");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Storages = new ObservableCollection<Storage>(JsonConvert.DeserializeObject<List<Storage>>(response.Content));
                SelectedStorage = Storages.FirstOrDefault();
            }
        }
        private async void AddProduct(object obj)
        {
            var productsListVM = new ProductListWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productsListVM));

            if (productsListVM.DialogResult == true)
            {
                if (DeliveryItems.Any(p => p.Electronic.ElectronicsId == productsListVM.SelectedElectronic.ElectronicsId) == false)
                {
                    DeliveryItems.Add(new DeliveryItem(productsListVM.SelectedElectronic, 0));
                }
                //await LoadElectronics();
                //CustomMessageBox.Show($"Заказ поиставщику номер упешно добавлен", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void RemoveProduct(object obj)
        {
            if (SelectedDeliveryItem != null)
            {
                DeliveryItems.Remove(SelectedDeliveryItem);
            }
        }
        private string GenerateDeliveryNumber()
        {
            Random random = new Random();
            string number = "";
            string keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < 10; i++)
            {
                number += keys[random.Next(0, keys.Length)];
            }

            number += $"{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}";

            return number;
        }

        private void DeliveryItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int id = 1;
            foreach (var item in DeliveryItems)
            {
                if (item.Id != id)
                {
                    item.Id = id;
                }
                id++;
            }
        }
    }
}
