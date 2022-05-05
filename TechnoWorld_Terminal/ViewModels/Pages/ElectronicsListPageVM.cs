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
using TechnoWorld_API.Models;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Models;
using TechnoWorld_Terminal.Services;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Services;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class ElectronicsListPageVM : PageVMBase
    {
        #region Fields
        private List<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;
        private ObservableCollection<Electronic> electronics;
        private ObservableCollection<Electronic> displayedElectronics;

        private List<ElectrnicsType> types;
        private List<Manufacturer> manufacturers;
        private SortParameter selectedSort;
        private Electronic selectedElectronic;
        private Category category;
        private Paginator paginator;
        private Visibility emptyVisibility;
        private string search;
        private bool orderByDescening;
        private int itemsPerPage;
        private int minPrice;
        private int maxPrice;
        private int lastPage;
        private int totalFilteredCount;
        #endregion

        public ElectronicsListPageVM()
        {
            InitializeFields();

            ClientService.Instance.HubConnection.On<string>("UpdateElectronics", (electronics) =>
            {
                GetElectronicsWithFilter();
            });
        }

        #region Properties
        public Paginator Paginator { get => paginator; set { paginator = value; OnPropertyChanged(); } }
        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand ToFirstPageCommand { get; set; }
        public RelayCommand ToLastPageCommand { get; set; }
        public RelayCommand ConfirmSortCommand { get; set; }
        public RelayCommand BackToCategoriesCommand { get; set; }
        public RelayCommand SelectElectrinicsCommand { get; set; }
        public RelayCommand OpenDetailInfoCommand { get; set; }
        public RelayCommand SortOrderChangedCommand { get; set; }
        public ObservableCollection<int> DisplayedPagesNumbers { get => displayedPagesNumbers; set { displayedPagesNumbers = value; OnPropertyChanged(); } }
        public List<int> PagesNumbers { get => pagesNumbers; set { pagesNumbers = value; OnPropertyChanged(); } }
        public ObservableCollection<Electronic> Electronics { get => electronics; set { electronics = value; OnPropertyChanged(); } }
        public ObservableCollection<Electronic> DisplayedElectronics { get => displayedElectronics; set { displayedElectronics = value; OnPropertyChanged(); } }
        public List<ElectrnicsType> Types
        {
            get => types;
            set
            {
                types = value;
                OnPropertyChanged();
            }
        }
        public List<Manufacturer> Manufacturers
        {
            get => manufacturers;
            set
            {
                manufacturers = value;
                OnPropertyChanged();
            }
        }

        public Category CurrentCategory
        {
            get => category;
            set
            {
                if (value.Id != category?.Id)
                {
                    category = value;
                    LoadData();
                }
                OnPropertyChanged();
            }
        }
        public ObservableCollection<SortParameter> SortParameters { get; set; }
        public SortParameter SelectedSort
        {
            get => selectedSort;
            set
            {
                selectedSort = value ?? SortParameters.FirstOrDefault(); OnPropertyChanged(); GetElectronicsWithFilter();
            }
        }

        public Electronic SelectedElectronic { get => selectedElectronic; set { selectedElectronic = value; OnPropertyChanged(); } }

        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }

        public string Search
        {
            get => search;
            set
            {
                search = value;
                OnPropertyChanged();
                GetElectronicsWithFilter();
            }
        }

        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; OnPropertyChanged(); }
        }
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
        public bool OrderByDescening { get => orderByDescening; set { orderByDescening = value; OnPropertyChanged(); } }
        #endregion

        private void InitializeFields()
        {
            lastPage = 1;
            itemsPerPage = 5;
            search = string.Empty;
            MaxPrice = 1000000;
            ChangePageCommand = new RelayCommand(ChangePage);
            ConfirmSortCommand = new RelayCommand(ConfirmSort);
            BackToCategoriesCommand = new RelayCommand(BackToCategories);
            OpenDetailInfoCommand = new RelayCommand(OpenDetailInfo);
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            EmptyVisibility = Visibility.Hidden;
        }


        private void OpenDetailInfo(object obj)
        {
            if (obj != null)
            {
                Electronic electronic = obj as Electronic;
                var vm = new ElectronicsDetailPageVM(electronic);
                PageNavigation.Navigate(vm);
            }

        }

        private void BackToCategories(object obj)
        {
            PageNavigation.Navigate(typeof(CategoriesPageVM));
        }

        private async void ConfirmSort(object obj)
        {
            GetElectronicsWithFilter();
        }

        private async void LoadData()
        {
            var task1 = LoadManufacturers();
            var task2 = LoadTypes();
            var task3 = LoadSortParams();

            await Task.WhenAll(task1, task2, task3);
            await Task.Run(LoadElectronics);


            OnPropertyChanged(nameof(SortParameters));
            OnPropertyChanged(nameof(SelectedSort));
        }

        private async Task LoadSortParams()
        {
            await Task.Run(() =>
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
            });
        }

        private async Task LoadTypes()
        {
            var response = (RestResponse)await ApiService.GetRequestWithParameter("api/ElectrnicsTypes", "categoryId", CurrentCategory.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(response.Content).OrderBy(p => p.Name).ToList();
                foreach (var item in Types)
                {
                    item.OnSelectionChanged += Type_OnSelectionChanged1;
                }
            }

        }
        private async Task LoadManufacturers()
        {
            var response = (RestResponse)await ApiService.GetRequestWithParameter("api/Manufacturers", "categoryId", CurrentCategory.Id);
            await Task.Run(() =>
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Manufacturers = JsonConvert.DeserializeObject<List<Manufacturer>>(response.Content).OrderBy(p => p.Name).ToList();
                    foreach (var item in Manufacturers)
                    {
                        item.OnSelectionChanged += Manufacturer_OnSelectionChanged;
                    }
                }
            });
        }
        private void Manufacturer_OnSelectionChanged(object sender, EventArgs e)
        {
            GetElectronicsWithFilter();
        }
        private void Type_OnSelectionChanged1(object sender, EventArgs e)
        {
            GetElectronicsWithFilter();
        }

        private async Task LoadElectronics()
        {
            await GetElectronicsWithFilter();
        }

        private async Task GetElectronicsWithFilter()
        {
            var request = await ApiService.GetRequestWithParameter("api/Electronics/TerminalFilter", "jsonFilter", JsonConvert.SerializeObject(
                new
                {
                    search = Search,
                    categoryId = CurrentCategory.Id,
                    listElectronicsTypeId = Types.Where(p => p.IsSelected).Select(p => p.TypeId),
                    listManufacturersId = Manufacturers.Where(p => p.IsSelected).Select(p => p.ManufacturerId),
                    sortParameter = SelectedSort.Property,
                    isAscending = SelectedSort.IsAscending,
                    isOfferedForSale = true,
                    minPrice = MinPrice,
                    maxPrice = MaxPrice,
                    currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
                    itemsPerPage = ItemsPerPage
                }));
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<FilteredElectronic>(request.Content);
                Electronics = new ObservableCollection<Electronic>(result.Electronics);
                totalFilteredCount = result.TotalFilteredCount;
                if (Paginator != null)
                {
                    await RefreshElectronics();
                }
                else
                {
                    Paginator = new Paginator(5, MaxPage());
                    await RefreshElectronics();
                }
                //dd.Add(JsonConvert.DeserializeObject<List<Electronic>>(request.Content));
            }

        }

        private async Task RefreshElectronics()
        {
            await Task.Run(() =>
            {
                var maxPage = MaxPage();

                Paginator.RefreshPages(maxPage == 0 ? 1 : maxPage);

                // var electronicsList = GetFilteredElectronics(Electronics);

                //Если после фильтрации у нас количество элементов 0, то выводим Пусто
                if (Electronics.Count() <= 0)
                {
                    EmptyVisibility = Visibility.Visible;
                    Paginator.SelectedPageNumber = 1;
                }
                else EmptyVisibility = Visibility.Hidden;

                //electronicsList = electronicsList.Skip((Paginator.SelectedPageNumber - 1) * itemsPerPage)
                //   .Take(itemsPerPage).ToList();

                OnPropertyChanged(nameof(DisplayedElectronics));
            });

        }
        private int MaxPage()
        {
            return (int)Math.Ceiling((float)totalFilteredCount / (float)ItemsPerPage);
        }
        private async void ChangePage(object obj)
        {
            if (obj != null)
            {
                if (Paginator != null)
                {

                    if (Convert.ToInt32(obj) == -1)
                    {
                        Paginator.ChangePage(1);
                    }
                    else if (Convert.ToInt32(obj) == 1)
                    {
                        var maxPage = MaxPage();
                        Paginator.ChangePage(MaxPage());
                    }
                }
                else return;
            }
            if (Paginator.DisplayedPagesNumbers.Count > 0)
            {
                await Task.Run(Paginator.RefrashPaginator);
                OnPropertyChanged(nameof(DisplayedElectronics));
                if (lastPage != Paginator.SelectedPageNumber)
                {
                    lastPage = Paginator.SelectedPageNumber;
                    await GetElectronicsWithFilter();
                }
                lastPage = Paginator.SelectedPageNumber;
            }
        }

        private async void SortOrderChanged(object obj)
        {
            await GetElectronicsWithFilter();
        }
    }
}
