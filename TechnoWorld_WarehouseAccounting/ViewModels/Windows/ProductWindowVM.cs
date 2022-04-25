using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class ProductWindowVM : BaseModalWindowVM
    {
        private bool isEdit;
        private ObservableCollection<Category> categories;
        private ObservableCollection<ElectrnicsType> electrnicsTypes;
        private ObservableCollection<Manufacturer> manufacturers;
        private Electronic CurrentElectronic { get; set; }

        public ProductWindowVM()
        {
            SaveCommand = new RelayCommand(Save);
            CurrentElectronic = new Electronic();
            isEdit = false;
            Task.Run(() => Initialize().Wait());
            Task.Run(() => LoadData().Wait());
        }
        public ProductWindowVM(Electronic electronic) : this()
        {
            Task.Run(() => InitializeFields(electronic).Wait());
        }
        public RelayCommand SaveCommand { get; set; }


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
        public ObservableCollection<ElectrnicsType> ElectronicsTypes
        {
            get { return electrnicsTypes; }
            set { electrnicsTypes = value; OnPropertyChanged(); }
        }
        public string Model { get => CurrentElectronic.Model; set { CurrentElectronic.Model = value; OnPropertyChanged(); } }
        public Category Category { get => CurrentElectronic.Type.Category; set { CurrentElectronic.Type.Category = value; CurrentElectronic.Type.CategoryId = value.Id; OnPropertyChanged(); } }
        public ElectrnicsType ElectrnicsType { get => CurrentElectronic.Type; set { CurrentElectronic.Type = value; CurrentElectronic.TypeId = value.TypeId; OnPropertyChanged(); } }
        public decimal Price { get => CurrentElectronic.Price; set { CurrentElectronic.Price = value; OnPropertyChanged(); } }
        public Manufacturer Manufacturer { get => CurrentElectronic.Manufacturer; set { CurrentElectronic.Manufacturer = value; CurrentElectronic.ManufactrurerId = value.ManufacturerId; OnPropertyChanged(); } }
        public string ManufacturerCountry { get => CurrentElectronic.ManufacturerСountry; set { CurrentElectronic.ManufacturerСountry = value; OnPropertyChanged(); } }
        public string Color { get => CurrentElectronic.Color; set { CurrentElectronic.Color = value; OnPropertyChanged(); } }
        public double Weight { get => CurrentElectronic.Weight; set { CurrentElectronic.Weight = value; OnPropertyChanged(); } }
        public string Description { get => CurrentElectronic.Description; set { CurrentElectronic.Description = value; OnPropertyChanged(); } }
        public byte[] Image { get => CurrentElectronic.Image; set { CurrentElectronic.Image = value; OnPropertyChanged(); } }

        private async Task Initialize()
        {

        }
        private async Task InitializeFields(Electronic electronic)
        {
            isEdit = true;
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
            var response = await ApiService.GetRequest("api/Categories");
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
            var response = await ApiService.GetRequest("api/ElectrnicsTypes/All");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ElectronicsTypes = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(response.Content));
                if (ElectrnicsType == null)
                {
                    ElectrnicsType = ElectronicsTypes.FirstOrDefault();
                }
            }
        }

        private async Task LoadManufacturers()
        {
            var response = await ApiService.GetRequest("api/Manufacturers/All");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Manufacturers = new ObservableCollection<Manufacturer>(JsonConvert.DeserializeObject<List<Manufacturer>>(response.Content));
                if (Manufacturer == null)
                {
                    Manufacturer = Manufacturers.FirstOrDefault();
                }
            }
        }

        private async void Save(object obj)
        {
            if (Validate())
            {
                if (isEdit)
                {
                    var response = await ApiService.PutRequest("api/Electronics", CurrentElectronic.ElectronicsId, CurrentElectronic);
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
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
                    var response = await ApiService.PostRequest("api/Electronics", CurrentElectronic);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
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

        private bool Validate()
        {
            return true;
        }
    }
}
