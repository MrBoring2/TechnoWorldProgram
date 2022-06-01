using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.Models;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class CartPageVM : BasePageVM
    {
        private Visibility emptyVisibility;
        private Visibility paymentVisibility;
        private Visibility deliveryAddressVisibility;
        // private MainAppWindow _context;
        //private decimal deliveryPrice;

        public CartPageVM()
        {
            CheckCartEmpty();
            BackToElectronicsCommand = new RelayCommand(BackToElectronics);
            CreateOrderCommand = new RelayCommand(CreateOrder);
            ClientService.Instance.Cart.CollectionChanged += Cart_CollectionChanged;
        }


        public RelayCommand CreateOrderCommand { get; set; }
        public RelayCommand BackToElectronicsCommand { get; set; }
        private void UpdatePayment()
        {
            OnPropertyChanged(nameof(ElectronicTotalPrice));
            OnPropertyChanged(nameof(TotalOrderPrice));
        }

        public Visibility EmptyVisibility
        {
            get => emptyVisibility;
            set
            {
                emptyVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility PaymentVisibility
        {
            get => paymentVisibility;
            set
            {
                paymentVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility DeliveryAddressVisibility
        {
            get => deliveryAddressVisibility;
            set
            {
                deliveryAddressVisibility = value;
                OnPropertyChanged();
            }
        }
        public decimal ElectronicTotalPrice => ClientService.Instance.Cart.Sum(p => p.TotalPrice);
        public decimal TotalOrderPrice => ElectronicTotalPrice;// + DeliveryPrice;
        public ObservableCollection<CartItem> CartItems => ClientService.Instance.Cart;

        private void CheckCartEmpty()
        {
            if (CartItems.Count <= 0)
            {
                EmptyVisibility = Visibility.Visible;
                PaymentVisibility = Visibility.Hidden;
            }
            else
            {
                EmptyVisibility = Visibility.Hidden;
                PaymentVisibility = Visibility.Visible;
            }
        }
        private async void CreateOrder(object obj)
        {
            var order = new Order
            {
                DateOfRegistration = DateTime.Now,
                OrderNumber = GenerateOrderNumber(),
                StatusId = 1,
                OrderElectronics = new List<OrderElectronic>()
            };
            foreach (var item in ClientService.Instance.Cart)
            {
                order.OrderElectronics.Add(new OrderElectronic { ElectronicsId = item.Electronic.ElectronicsId, Count = item.Amount });
            }

            var response = await ApiService.Instance.PostRequest("api/Orders", order);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                CustomNotificationManager.ShowNotification(new NotificationContent() { Title = "Оповещение", Message = $"Заказ успешно оформлен. Подойдите к кассе. Номер заказа: {order.OrderNumber}", Type = NotificationType.Success }, "MainNotificationArea");
                ClientService.Instance.Cart.Clear();
                MaterialNotification.Show("Заказ оформлен", $"Ваш номер заказа {order.OrderNumber}, подойдите с ним к кассе.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                CustomNotificationManager.ShowNotification(new NotificationContent() { Title = "Ошибка", Message = JsonConvert.DeserializeObject<string>(response.Content), Type = NotificationType.Error }, "MainNotificationArea");
            }
        }

        private string GenerateOrderNumber()
        {
            Random random = new Random();
            string number = "";
            string keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            for (int i = 0; i < 10; i++)
            {
                number += keys[random.Next(0, keys.Length)];
            }
            return number;
        }

        private void BackToElectronics(object obj)
        {
            if (PageNavigation.IsPageExist(typeof(ElectronicsListPageVM)))
            {
                PageNavigation.Navigate(typeof(ElectronicsListPageVM));
            }
            else
            {
                PageNavigation.Navigate(typeof(ElectronicsListPageVM));
            }
        }


        private void Cart_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in CartItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
            foreach (var item in CartItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            CheckCartEmpty();
            UpdatePayment();
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdatePayment();
        }
    }
}
