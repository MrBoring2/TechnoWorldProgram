using System.Linq;
using TechoWorld_DataModels_v2;

using TechnoWorld_Terminal.Services;
using Notification.Wpf;
using WPF_Helpers.Common;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class ElectronicsDetailPageVM : BasePageVM
    {
        private string model;
        private Manufacturer manfacturer;
        private ElectrnicsType electrnicsType;
        private decimal price;
        private int amountInStorage;
        private string description;
        private byte[] image;
        private string color;
        private string manufacturerCountry;
        private int harantyMonth;
        private double weight;
        public ElectronicsDetailPageVM()
        {
        }
        public ElectronicsDetailPageVM(Electronic electronic)
        {
            CurrentElectronic = electronic;
            InitializeFields();
            BackToElectronicsCommand = new RelayCommand(BackToElectronics);
            AddToCardCommand = new RelayCommand(AddToCard);
        }

        public RelayCommand BackToElectronicsCommand { get; set; }
        public RelayCommand AddToCardCommand { get; set; }
        private Electronic CurrentElectronic { get; set; }
        public string Model
        {
            get { return model; }
            set { model = value; OnPropertyChanged(); }
        }

        public Manufacturer Manufacturer
        {
            get { return manfacturer; }
            set { manfacturer = value; OnPropertyChanged(); }
        }
        public ElectrnicsType ElectronicType
        {
            get { return electrnicsType; }
            set { electrnicsType = value; OnPropertyChanged(); }
        }
        public decimal SalePrice
        {
            get { return price; }
            set { price = value; OnPropertyChanged(); }
        }
        public int AmountInStorage
        {
            get { return amountInStorage; }
            set { amountInStorage = value; OnPropertyChanged(); }
        }
        public string Description
        {
            get { return description; }
            set { description = value; OnPropertyChanged(); }
        }
        public byte[] Image
        {
            get => image;
            set
            {
                image = value;
                OnPropertyChanged();
            }
        }
        public string Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged();
            }
        }

        public string ManufacturerCountry
        {
            get => manufacturerCountry;
            set
            {
                manufacturerCountry = value;
                OnPropertyChanged();
            }
        }

        public int HarantyMonth
        {
            get => harantyMonth;
            set
            {
                harantyMonth = value;
                OnPropertyChanged();
            }
        }

        public double Weight
        {
            get => weight;
            set
            {
                weight = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Загрузка данных на форму
        /// </summary>
        private void InitializeFields()
        {
            Model = CurrentElectronic.Model;
            Manufacturer = CurrentElectronic.Manufacturer;
            ElectronicType = CurrentElectronic.Type;
            SalePrice = CurrentElectronic.SalePrice;
            AmountInStorage = CurrentElectronic.AmountInStoragesWithReservation;
            Description = CurrentElectronic.Description;
            Image = CurrentElectronic.Image;
            Color = CurrentElectronic.Color;
            Weight = CurrentElectronic.Weight;
            ManufacturerCountry = CurrentElectronic.ManufacturerСountry;
        }

        /// <summary>
        /// Назад к списку электроники
        /// </summary>
        /// <param name="obj"></param>
        private void BackToElectronics(object obj)
        {
            PageNavigation.Navigate(typeof(ElectronicsListPageVM));
        }

        /// <summary>
        /// Добавить в корзину
        /// </summary>
        /// <param name="obj"></param>
        private void AddToCard(object obj)
        {

            if (ClientService.Instance.Cart.FirstOrDefault(p => p.Electronic.ElectronicsId == CurrentElectronic.ElectronicsId) == null)
            {
                if (CurrentElectronic.AmountInStoragesWithReservation > 0)
                {
                    ClientService.Instance.Cart.Add(new Models.CartItem(CurrentElectronic));
                    CustomNotificationManager.ShowNotification(new NotificationContent() { Title = "Оповещение", Message = "Товар добавлен в корзину", Type = NotificationType.Information }, "ElectronicNotificationArea");
                }
                else
                {
                    CustomNotificationManager.ShowNotification(new NotificationContent() { Title = "Внимание", Message = "Товар нет в наличии", Type = NotificationType.Warning }, "ElectronicNotificationArea");
                }
            }
            else
            {
                CustomNotificationManager.ShowNotification(new NotificationContent() { Title = "Внимание", Message = "Товар уже находится в корзине", Type = NotificationType.Warning }, "ElectronicNotificationArea");
            }
        }
    }
}
