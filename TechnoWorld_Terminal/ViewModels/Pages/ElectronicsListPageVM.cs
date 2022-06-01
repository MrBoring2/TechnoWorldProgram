using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using TechnoWorld_Terminal.Models;
using TechnoWorld_Terminal.Services;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_Helpers.Services;
using WPF_VM_Abstractions;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class ElectronicsListPageVM : ListEntitiesPageVM<Electronic, FilteredElectronic>
    {
        #region Fields   
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<ElectrnicsType> types;
        private ObservableCollection<Manufacturer> manufacturers;
        private SortParameter selectedSort;
        private Electronic selectedElectronic;
        private int categoryId;
        private int minPrice;
        private int maxPrice;
        #endregion

        public ElectronicsListPageVM() : base(5)
        {
            InitializeFields(1);

            ApiService.Instance.GetHubConnection.On<string>("UpdateElectronics", async (electronics) =>
            {
                await GetWithFilter();
            });
        }
        public ElectronicsListPageVM(int categoryId) : base(5)
        {
            InitializeFields(categoryId);

            ApiService.Instance.GetHubConnection.On<string>("UpdateElectronics", async (electronics) =>
            {
                await GetWithFilter();
            });
        }

        #region Properties     
        public RelayCommand ConfirmSortCommand { get; set; }
        public RelayCommand BackToCategoriesCommand { get; set; }
        public RelayCommand SelectElectrinicsCommand { get; set; }
        public RelayCommand OpenDetailInfoCommand { get; set; }
        public ObservableCollection<ElectrnicsType> Types
        {
            get => types;
            set
            {
                types = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => manufacturers;
            set
            {
                manufacturers = value;
                OnPropertyChanged();
            }
        }

        public int CurrentCategoryId
        {
            get => categoryId;
            set
            {
                if (value != categoryId)
                {
                    categoryId = value;
                    LoadData();
                }
                OnPropertyChanged();
            }
        }

        public Electronic SelectedElectronic { get => selectedElectronic; set { selectedElectronic = value; OnPropertyChanged(); } }


        public int MinPrice
        {
            get { return minPrice; }
            set { minPrice = value; OnPropertyChanged(); }
        }
        public int MaxPrice
        {
            get { return maxPrice; }
            set { maxPrice = value; OnPropertyChanged(); }
        }

        protected override string UrlApi => "api/Electronics/TerminalFilter";

        public override ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }

        protected override object FilterParam => new
        {
            search = Search,
            categoryId = CurrentCategoryId,
            listElectronicsTypeId = Types == null ? null : Types.Where(p => p.IsSelected).ToList().Select(p => p.TypeId),
            listManufacturersId = Manufacturers == null ? null : Manufacturers.Where(p => p.IsSelected).ToList().Select(p => p.ManufacturerId),
            sortParameter = SelectedSort.Property,
            isAscending = SelectedSort.IsAcsending,
            isOfferedForSale = true,
            minPrice = MinPrice,
            maxPrice = MaxPrice,
            currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
            itemsPerPage = ItemsPerPage
        };
        #endregion

        private void InitializeFields(int categoryId)
        {
            CurrentCategoryId = categoryId;
            MaxPrice = 1000000;
            ConfirmSortCommand = new RelayCommand(ConfirmSort);
            BackToCategoriesCommand = new RelayCommand(BackToCategories);
            OpenDetailInfoCommand = new RelayCommand(OpenDetailInfo);
            EmptyVisibility = Visibility.Hidden;
        }


        private void OpenDetailInfo(object obj)
        {
            if (obj != null)
            {
                Electronic electronic = obj as Electronic;
                var vm = new ElectronicsDetailPageVM(electronic);
                PageNavigation.NavigateToNewPage(vm);
            }

        }

        private void BackToCategories(object obj)
        {
            PageNavigation.Navigate(typeof(CategoriesPageVM));
        }

        private async void ConfirmSort(object obj)
        {
            await GetWithFilter();
        }

        private async void LoadData()
        {
            LoadSortParams();

            await GetWithFilter();
            await LoadManufacturers();
            await LoadTypes();

            OnPropertyChanged(nameof(SortParameters));
            OnPropertyChanged(nameof(SelectedSort));
        }

        private void LoadSortParams()
        {

            SortParameters = new ObservableCollection<SortParameter>
                {
                new SortParameter("Модель", "Model"),
                new SortParameter("Цена", "SalePrice"),
                new SortParameter("Производитель", "ManufacturerName")
                };

            selectedSort = SortParameters.FirstOrDefault();
            OnPropertyChanged(nameof(SortParameters));
            OnPropertyChanged(nameof(SelectedSort));

        }

        private async Task LoadTypes()
        {
            var response = (RestResponse)await ApiService.Instance.GetRequestWithParameter("api/ElectrnicsTypes", "categoryId", CurrentCategoryId);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Types = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(response.Content).OrderBy(p => p.Name));
                foreach (var item in Types)
                {
                    item.OnSelectionChanged += Type_OnSelectionChanged;
                }
            }

        }
        private async Task LoadManufacturers()
        {
            var response = (RestResponse)await ApiService.Instance.GetRequestWithParameter("api/Manufacturers", "categoryId", CurrentCategoryId);
            await Task.Run(() =>
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Manufacturers = new ObservableCollection<Manufacturer>(JsonConvert.DeserializeObject<List<Manufacturer>>(response.Content).OrderBy(p => p.Name));
                    foreach (var item in Manufacturers)
                    {
                        item.OnSelectionChanged += Manufacturer_OnSelectionChanged;
                    }
                }
            });
        }
        private async void Manufacturer_OnSelectionChanged(object sender, EventArgs e)
        {
            var a = Manufacturers.Where(p => p.IsSelected).ToList();
            await GetWithFilter();
        }
        private async void Type_OnSelectionChanged(object sender, EventArgs e)
        {
            var a = Types.Where(p => p.IsSelected).ToList();
            await GetWithFilter();
        }
    }
}
