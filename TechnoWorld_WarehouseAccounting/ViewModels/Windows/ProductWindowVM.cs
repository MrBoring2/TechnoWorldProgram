﻿using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
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
        private Category category;
        private ObservableCollection<Category> categories;
        private ObservableCollection<ElectrnicsType> electrnicsTypes;
        private ObservableCollection<Manufacturer> manufacturers;
        private Electronic CurrentElectronic { get; set; }

        public ProductWindowVM()
        {
            Initialize();
            LoadData();

            ValidationMessageSetter(Weight, nameof(Weight));
            ValidationMessageSetter(Model == null ? "" : Model, nameof(Model));
            ValidationMessageSetter(SalePrice, nameof(SalePrice));
            ValidationMessageSetter(PurchasePrice, nameof(PurchasePrice));
            ValidationMessageSetter(Color == null ? "" : Color, nameof(Color));
            ValidationMessageSetter(ManufacturerCountry == null ? "" : ManufacturerCountry, nameof(ManufacturerCountry));
            ValidationMessageSetter(Description == null ? "" : Description, nameof(Description));
        }


        public ProductWindowVM(Electronic electronic)
        {
            Task.Run(() => Initialize()).Wait();
            Task.Run(() => LoadData().Wait());

            Task.Run(() => InitializeFields(electronic)).Wait();
        }
        public void TextChanged(object sender, TextChangedEventArgs e)
        {
            //if ((sender as TextBox).Text == "")
            //{
            //    e.Handled = true;
            //    (sender as TextBox).Text = "0";
            //}
        }
        public void IsAllowedInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var cursorIndex = (sender as TextBox).CaretIndex;
            var text = (sender as TextBox).Text;
            if (sender is TextBox textBox && !e.Text.All(ch => char.IsDigit(ch)))
            {
                e.Handled = true;
            }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand CreateManufacturerCommand { get; set; }
        public RelayCommand CreateCategoryCommand { get; set; }
        public RelayCommand CreateElectronicsTypeCommand { get; set; }
        public RelayCommand LoadImageCommand { get; set; }
        public RelayCommand CheckNumerikTextBoxCommand { get; set; }

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
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Длина поля Модель слишком большая: максимум {1} символов")]
        public string Model { get => CurrentElectronic.Model; set { CurrentElectronic.Model = value; ValidationMessageSetter(value); } }
        public Category Category
        {
            get => category;
            set
            {

                category = value;
                if (AllElectronicsTypes != null)
                {
                    ElectronicsTypes = new ObservableCollection<ElectrnicsType>(AllElectronicsTypes.Where(p => p.CategoryId == Category.Id).ToList());
                    if (ElectrnicsType == null)
                    {
                        ElectrnicsType = ElectronicsTypes.FirstOrDefault();
                    }
                }
                //CurrentElectronic.TypeId = ElectrnicsType.TypeId;
                //CurrentElectronic.Type.CategoryId = ElectrnicsType.CategoryId;
                OnPropertyChanged();
            }

        }


        public ElectrnicsType ElectrnicsType
        {
            get => CurrentElectronic.Type;
            set
            {
                CurrentElectronic.Type = value;
                CurrentElectronic.TypeId = CurrentElectronic.Type != null ? CurrentElectronic.Type.TypeId : 0;
                OnPropertyChanged();
            }
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [Range(minimum: 1, maximum: 3000000, ErrorMessage = "Указана недопустимая цена продажи: минимум {1}, максимум {2}")]
        public decimal SalePrice
        {
            get => CurrentElectronic.SalePrice;
            set { CurrentElectronic.SalePrice = Convert.ToDecimal(value); ValidationMessageSetter(value); }
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [Range(minimum: 1, maximum: 3000000, ErrorMessage = "Указан недопустимая цена закупки: минимум {1}, максимум {2}")]
        public decimal PurchasePrice { get => CurrentElectronic.PurchasePrice; set { CurrentElectronic.PurchasePrice = Convert.ToDecimal(value); ValidationMessageSetter(value); } }
        public Manufacturer Manufacturer { get => CurrentElectronic.Manufacturer; set { CurrentElectronic.Manufacturer = value; CurrentElectronic.ManufactrurerId = value.ManufacturerId; OnPropertyChanged(); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Длина поля Страна производитель слишком большая: максимум {1} символов")]
        public string ManufacturerCountry { get => CurrentElectronic.ManufacturerСountry; set { CurrentElectronic.ManufacturerСountry = value; ValidationMessageSetter(value); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100, ErrorMessage = "Длина поля Цвет слишком большая: максимум {1} символов")]
        public string Color { get => CurrentElectronic.Color; set { CurrentElectronic.Color = value; ValidationMessageSetter(value); } }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [Range(minimum: 0.01, maximum: 10000, ErrorMessage = "Указан недопустимый вес: минимум {1}, максимум {2}")]
        public double Weight
        {
            get => CurrentElectronic.Weight;
            set
            {
                if (CurrentElectronic.Weight != value)
                {
                    CurrentElectronic.Weight = value;
                    CurrentElectronic.Weight = Convert.ToDouble(value);
                    ValidationMessageSetter(value);
                }
            }
        }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не должно быть пустым")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(2000, ErrorMessage = "Длина поля Описание слишком большая: максимум {1} символов")]
        public string Description { get => CurrentElectronic.Description; set { CurrentElectronic.Description = value; ValidationMessageSetter(value); } }
        public byte[] Image { get => CurrentElectronic.Image; set { CurrentElectronic.Image = value; OnPropertyChanged(); } }
        public bool IsOfferedForSale { get => CurrentElectronic.IsOfferedForSale; set { CurrentElectronic.IsOfferedForSale = value; OnPropertyChanged(); } }
        private void Initialize()
        {
            SaveCommand = new RelayCommand(Save);
            LoadImageCommand = new RelayCommand(LoadImage);
            CancelCommand = new RelayCommand(Cancel);
            CreateManufacturerCommand = new RelayCommand(CreateManufacturer);
            CreateElectronicsTypeCommand = new RelayCommand(CreateElectronicsType);
            CreateCategoryCommand = new RelayCommand(CreateCategory);
            CurrentElectronic = new Electronic();
            IsAdd = true;
        }
        private async Task InitializeFields(Electronic electronic)
        {
            CurrentElectronic = (Electronic)electronic.Clone();
            Category = CurrentElectronic.Type.Category;
            //ElectrnicsType = CurrentElectronic.Type;
            //Type = 
            IsAdd = false;
            OnPropertyChanged(nameof(Image));

            OnPropertyChanged(nameof(ElectrnicsType));
            OnPropertyChanged(nameof(Manufacturer));
            OnPropertyChanged(nameof(Category));

            ValidationMessageSetter(Weight, nameof(Weight));
            ValidationMessageSetter(Model == null ? "" : Model, nameof(Model));
            ValidationMessageSetter(SalePrice, nameof(SalePrice));
            ValidationMessageSetter(PurchasePrice, nameof(PurchasePrice));
            ValidationMessageSetter(Color == null ? "" : Color, nameof(Color));
            ValidationMessageSetter(ManufacturerCountry == null ? "" : ManufacturerCountry, nameof(ManufacturerCountry));
            ValidationMessageSetter(Description == null ? "" : Description, nameof(Description));

        }
        private async Task LoadData()
        {
            var task1 = LoadCategories();
            var task2 = LoadTypes();
            var task3 = LoadManufacturers();
            await Task.WhenAll(task1, task2, task3);
        }

        private async Task LoadCategories()
        {
            var response = await ApiService.Instance.GetRequest("api/Categories");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<Category>(JsonConvert.DeserializeObject<List<Category>>(response.Content).OrderBy(p => p.Name));
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
                AllElectronicsTypes = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(response.Content).OrderBy(p => p.Name));
                if (ElectrnicsType == null)
                {
                    ElectrnicsType = AllElectronicsTypes.FirstOrDefault();
                }
                //if (Category != null)
                //{
                //    ElectronicsTypes = new ObservableCollection<ElectrnicsType>(AllElectronicsTypes.Where(p => p.CategoryId == Category.Id).ToList());
                //    ElectrnicsType = ElectronicsTypes.FirstOrDefault();
                //}
            }
        }

        private async Task LoadManufacturers()
        {
            var response = await ApiService.Instance.GetRequest("api/Manufacturers/All");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Manufacturers = new ObservableCollection<Manufacturer>(JsonConvert.DeserializeObject<List<Manufacturer>>(response.Content).OrderBy(p => p.Name));
                if (Manufacturer == null)
                {
                    Manufacturer = Manufacturers.FirstOrDefault();
                }
            }
        }

        private async void CreateManufacturer(object obj)
        {
            ManufacturerWindowVM manufacturerWindow = new ManufacturerWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(manufacturerWindow));
            if (manufacturerWindow.DialogResult == true)
            {
                await LoadManufacturers();
            }
        }

        private async void CreateCategory(object obj)
        {
            CategoryWindowVM categoryWindow = new CategoryWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(categoryWindow));
            if (categoryWindow.DialogResult == true)
            {
                await LoadCategories();
            }
        }
        private async void CreateElectronicsType(object obj)
        {
            ElectronicsTypeWindowVM electronicsTypeWindow = new ElectronicsTypeWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(electronicsTypeWindow));
            if (electronicsTypeWindow.DialogResult == true)
            {
                await LoadTypes();
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
            if (GetErrorsCount > 0)
            {
                MaterialNotification.Show("Внимание", "Не все поля заполены верно!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
            }
            else
            {
                if (PurchasePrice > SalePrice)
                {
                    MaterialNotification.Show("Внимание", "Цена закупки не должна быть больше цены продажи!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                    return;
                }

                if (isAdd)
                {
                    var response = await ApiService.Instance.PostRequest("api/Electronics", CurrentElectronic);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        DialogResult = true;
                    }
                    else
                    {
                        MaterialNotification.Show("Ошибка", JsonConvert.DeserializeObject<string>(response.Content), MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
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
                        MaterialNotification.Show("Ошибка", JsonConvert.DeserializeObject<string>(response.Content), MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }

                }
            }
        }

        private void Cancel(object obj)
        {
            DialogResult = false;
        }
    }
}
