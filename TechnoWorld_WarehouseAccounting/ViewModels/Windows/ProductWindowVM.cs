
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class ProductWindowVM : BaseModalWindowVM
    {
        private bool isAdd;
        private ObservableCollection<Category> categories;
        private ObservableCollection<ElectrnicsType> electrnicsTypes;
        private ObservableCollection<Manufacturer> manufacturers;
        private Electronic CurrentElectronic { get; set; }

        public ProductWindowVM()
        {
            SaveCommand = new RelayCommand(Save);
            LoadImageCommand = new RelayCommand(LoadImage);
            CancelCommand = new RelayCommand(Cancel);
            CurrentElectronic = new Electronic();
            IsAdd = true;

            Task.Run(() => Initialize());
            Task.Run(() => LoadData());
        }


        public ProductWindowVM(Electronic electronic) : this()
        {
            Task.Run(() => InitializeFields(electronic).Wait());
        }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand LoadImageCommand { get; set; }

        public ObservableCollection<Category> Categories
        {
            get { return categories; }
            set { categories = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get { return manufacturers; }
            set { manufacturers = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ElectrnicsType> AllElectronicsTypes { get; set; }
        public ObservableCollection<ElectrnicsType> ElectronicsTypes
        {
            get { return electrnicsTypes; }
            set { electrnicsTypes = value; OnPropertyChanged(); }
        }
        public bool IsAdd { get => isAdd; set { isAdd = value; OnPropertyChanged(); } }
        public string Model { get => CurrentElectronic.Model; set { CurrentElectronic.Model = value; OnPropertyChanged(); } }
        public Category Category
        {
            get => CurrentElectronic.Type == null ? null : CurrentElectronic.Type.Category;
            set
            {
                if (CurrentElectronic.Type != null)
                {
                    CurrentElectronic.Type.Category = value;
                    //CurrentElectronic.Type.CategoryId = value.Id;
                   // ElectrnicsType = null;
                    ElectronicsTypes = new ObservableCollection<ElectrnicsType>(AllElectronicsTypes.Where(p => p.CategoryId == Category.Id).ToList());
                    ElectrnicsType = ElectronicsTypes.FirstOrDefault();
                }
                OnPropertyChanged();
            }
        }
        public ElectrnicsType ElectrnicsType { get => CurrentElectronic.Type; set { CurrentElectronic.Type = value; OnPropertyChanged(); } }
        public decimal SalePrice { get => CurrentElectronic.SalePrice; set { CurrentElectronic.SalePrice = value; OnPropertyChanged(); } }
        public decimal PurchasePrice { get => CurrentElectronic.PurchasePrice; set { CurrentElectronic.PurchasePrice = value; OnPropertyChanged(); } }
        public Manufacturer Manufacturer { get => CurrentElectronic.Manufacturer; set { CurrentElectronic.Manufacturer = value; CurrentElectronic.ManufactrurerId = value.ManufacturerId; OnPropertyChanged(); } }
        public string ManufacturerCountry { get => CurrentElectronic.ManufacturerСountry; set { CurrentElectronic.ManufacturerСountry = value; OnPropertyChanged(); } }
        public string Color { get => CurrentElectronic.Color; set { CurrentElectronic.Color = value; OnPropertyChanged(); } }
        public double Weight { get => CurrentElectronic.Weight; set { CurrentElectronic.Weight = value; OnPropertyChanged(); } }
        public string Description { get => CurrentElectronic.Description; set { CurrentElectronic.Description = value; OnPropertyChanged(); } }
        public byte[] Image { get => CurrentElectronic.Image; set { CurrentElectronic.Image = value; OnPropertyChanged(); } }
        public bool IsOfferedForSale { get => CurrentElectronic.IsOfferedForSale; set { CurrentElectronic.IsOfferedForSale = value; OnPropertyChanged(); } }

        private async Task Initialize()
        {

        }
        private async Task InitializeFields(Electronic electronic)
        {
            IsAdd = false;
            CurrentElectronic = electronic;
        }
        private async Task LoadData()
        {
            await LoadCategories();
            await LoadTypes();
            await LoadManufacturers();
        }

        private async Task LoadCategories()
        {
            var response = await ApiService.Instance.GetRequest("api/Categories");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<Category>(JsonConvert.DeserializeObject<List<Category>>(response.Content));
                if (Category == null)
                {
                    Category = Categories.FirstOrDefault();
                }
            }
        }
        private async Task LoadTypes()
        {
            var response = await ApiService.Instance.GetRequest("api/ElectrnicsTypes/All");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AllElectronicsTypes = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(response.Content));
                if (ElectrnicsType == null)
                {
                    ElectrnicsType = AllElectronicsTypes.FirstOrDefault();
                }
            }
        }

        private async Task LoadManufacturers()
        {
            var response = await ApiService.Instance.GetRequest("api/Manufacturers/All");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Manufacturers = new ObservableCollection<Manufacturer>(JsonConvert.DeserializeObject<List<Manufacturer>>(response.Content));
                if (Manufacturer == null)
                {
                    Manufacturer = Manufacturers.FirstOrDefault();
                }
            }
        }

        private void LoadImage(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы изображения |*.jpg;*.jpeg;*.png;*.gif;*.tif;";
            if (openFileDialog.ShowDialog() == true)
            {
                Image = File.ReadAllBytes(openFileDialog.FileName);
            }
        }
        private async void Save(object obj)
        {
            if (Validate())
            {
                if (isAdd)
                {
                    var response = await ApiService.Instance.PostRequest("api/Electronics", CurrentElectronic);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        CustomMessageBox.Show(JsonConvert.DeserializeObject<string>(response.Content), "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    var response = await ApiService.Instance.PutRequest("api/Electronics", CurrentElectronic.ElectronicsId, CurrentElectronic);
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        CustomMessageBox.Show(JsonConvert.DeserializeObject<string>(response.Content), "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                }
            }
        }

        private void Cancel(object obj)
        {
            DialogResult = false;
        }
        private bool Validate()
        {
            if (string.IsNullOrEmpty(Model))
            {
                CustomMessageBox.Show("Поле модель не заполнено!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else if (SalePrice <= 0)
            {
                CustomMessageBox.Show("Цена не должна быть отрицательной!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else if (string.IsNullOrEmpty(Model))
            {
                CustomMessageBox.Show("Поле страна производитель не заполнено!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else if (string.IsNullOrEmpty(Color))
            {
                CustomMessageBox.Show("Поле цвет не заполнено!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else if (Weight <= 0)
            {
                CustomMessageBox.Show("Вес не должен быть отрицательным!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else if (string.IsNullOrEmpty(Description))
            {
                CustomMessageBox.Show("Поле описание не заполнено!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
