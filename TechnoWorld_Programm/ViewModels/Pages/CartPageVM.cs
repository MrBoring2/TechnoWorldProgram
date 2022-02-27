using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Models;
using TechnoWorld_Terminal.Services;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class CartPageVM : PageVMBase
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
            ClientService.Instance.Cart.CollectionChanged += Cart_CollectionChanged;
        }

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


        //public decimal DeliveryPrice
        //{
        //    get => deliveryPrice;
        //    set
        //    {
        //        deliveryPrice = value;
        //        OnPropertyChanged();
        //    }
        //}
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
        private void BackToElectronics(object obj)
        {
            PageNavigation.Navigate(typeof(ElectronicsListPageVM));
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
