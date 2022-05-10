using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels_v2;
using TechnoWorld_API.Models;
using Microsoft.AspNetCore.SignalR.Client;
using WPF_VM_Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_Helpers;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using TechnoWorld_Notification.Enums;
using TechnoWorld_Notification;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class ProductManagementPageVM : ListEntitiesPageVM<Electronic, FilteredElectronic>
    {
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<ItemWithTitle<Category>> categories;
        private ObservableCollection<ItemWithTitle<ElectrnicsType>> electronicsTypes { get; set; }
        private ObservableCollection<ItemWithTitle<bool?>> forDisplayList { get; set; }
        private ItemWithTitle<Category> selectedCategory;
        private ItemWithTitle<ElectrnicsType> selectedType;
        private ItemWithTitle<bool?> selectedDisplay;
        private bool isCategorySelected;
        private Visibility editAddVisibility;
        public event EventHandler onElectronicSelected;
        public ProductManagementPageVM() : base(15)
        {
            Initialize();
            LoadData();
            ApiService.Instance.GetHubConnection.On<string>("UpdateElectronics", async (deliveries) =>
            {
                await GetWithFilter();
            });
        }
        public ProductManagementPageVM(bool isModal = false) : this()
        {
            if (isModal)
            {
                EditAddVisibility = Visibility.Collapsed;
            }
            else
            {
                EditAddVisibility = Visibility.Visible;
            }
        }


        public RelayCommand OpenProductWindowCommand { get; set; }
        public RelayCommand OpenEditProductWindowCommand { get; set; }
        public ObservableCollection<ItemWithTitle<Category>> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public ObservableCollection<ElectrnicsType> AllElectronicsTypes { get; set; }
        public override ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<bool?>> ForDisplayList { get => forDisplayList; set { forDisplayList = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<ElectrnicsType>> ElectronicsTypes
        {
            get => electronicsTypes;
            set
            {
                electronicsTypes = value;
                OnPropertyChanged();
            }
        }
        public bool IsCategorySelected
        {
            get => isCategorySelected;
            set
            {
                isCategorySelected = value;

                if (isCategorySelected == true)
                {

                    ElectronicsTypes = new ObservableCollection<ItemWithTitle<ElectrnicsType>>(AllElectronicsTypes.Where(p => p.Category.Name == SelectedCategory.Item.Name).Select(p => new ItemWithTitle<ElectrnicsType>(p, p.Name)));
                    ElectronicsTypes.Insert(0, new ItemWithTitle<ElectrnicsType>(null, "Все"));
                    selectedType = ElectronicsTypes.FirstOrDefault();
                    OnPropertyChanged(nameof(SelectedType));
                }
                else
                {
                    ElectronicsTypes?.Clear();
                    selectedType = new ItemWithTitle<ElectrnicsType>(null, "Все");
                }
                OnPropertyChanged();
            }
        }


        public ItemWithTitle<bool?> SelectedDisplay
        {
            get { return selectedDisplay; }
            set { selectedDisplay = value; OnPropertyChanged(); GetWithFilter(); }
        }

        public ItemWithTitle<Category> SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                if (selectedCategory.Title != "Все")
                {
                    IsCategorySelected = true;
                }
                else
                {
                    IsCategorySelected = false;
                }
                OnPropertyChanged();
                GetWithFilter();
            }
        }
        public ItemWithTitle<ElectrnicsType> SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                OnPropertyChanged();
                GetWithFilter();
            }
        }
        public Visibility EditAddVisibility { get => editAddVisibility; set { editAddVisibility = value; OnPropertyChanged(); } }
        protected override string UrlApi => "api/Electronics/Filter";

        protected override object FilterParam
        {
            get
            {
                return new
                {
                    search = Search,
                    categoryId = SelectedCategory.Item == null ? 0 : SelectedCategory.Item.Id,
                    electronicsTypeId = SelectedType == null || SelectedType.Item == null ? 0 : SelectedType.Item.TypeId,
                    sortParameter = SelectedSort.Property,
                    isAscending = SelectedSort.IsAcsending,
                    isOfferedForSale = SelectedDisplay.Item,
                    currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
                    itemsPerPage = ItemsPerPage
                };
            }
        }

        protected async override void AfterSetSelectedSort(SortParameter value)
        {
            await GetWithFilter();
        }

        protected async override void AfterSetSearch(string value)
        {
            await GetWithFilter();
        }

        private void Initialize()
        {
            
            OpenProductWindowCommand = new RelayCommand(OpenProductWindow);
            OpenEditProductWindowCommand = new RelayCommand(OpenEditProductWindow);
            IsCategorySelected = false;
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Модель", "Model"),
                new SortParameter("Цена продажи", "SalePrice"),
                new SortParameter("Цена закупки", "PurchasePrice")
            };
            ForDisplayList = new ObservableCollection<ItemWithTitle<bool?>>
            {
                new ItemWithTitle<bool?>(null, "Все"),
                new ItemWithTitle<bool?>(true, "Выстевлены на продажу"),
                new ItemWithTitle<bool?>(false, "Сняты с продажи")
            };

            selectedDisplay = ForDisplayList.FirstOrDefault(p => p.Item == true);

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedDisplay));
            OnPropertyChanged(nameof(SelectedSort));
        }


        private async void LoadData()
        {
            await LoadCategories();
            await LoadTypes();
            await GetWithFilter();
        }


        private async Task LoadCategories()
        {
            var request = await ApiService.Instance.GetRequest("api/Categories");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<ItemWithTitle<Category>>(JsonConvert.DeserializeObject<List<Category>>(request.Content).Select(p => new ItemWithTitle<Category>(p, p.Name)));
                Categories.Insert(0, new ItemWithTitle<Category>(null, "Все"));
                selectedCategory = Categories.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        private async Task LoadTypes()
        {
            var request = await ApiService.Instance.GetRequest("api/ElectrnicsTypes/All");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AllElectronicsTypes = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(request.Content));
                selectedType = new ItemWithTitle<ElectrnicsType>(null, "Все");
            }
        }



        private async void OpenProductWindow(object obj)
        {
            ProductWindowVM productWindowVM = new ProductWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productWindowVM));
            if (productWindowVM.DialogResult == true)
            {
                MaterialNotification.Show("Оповещение", $"Товар {productWindowVM.Model} упешно добавлен.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
            }
        }
        private async void OpenEditProductWindow(object obj)
        {
            if (EditAddVisibility != Visibility.Collapsed)
            {
                ProductWindowVM productWindowVM;
                if (SelectedEntity == null)
                {
                    MaterialNotification.Show("Оповещение", $"Сначала выберите товар!", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                }
                else
                {
                    productWindowVM = new ProductWindowVM(SelectedEntity);

                    await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(productWindowVM));

                    if (productWindowVM.DialogResult == true)
                    {
                        MaterialNotification.Show("Оповещение", $"Товар {productWindowVM.Model} упешно изменён.", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
                    }
                }
            }
            else
            {
                onElectronicSelected.Invoke(this, new EventArgs());
            }
        }

    }
}
