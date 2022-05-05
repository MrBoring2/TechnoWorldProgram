using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class ProductDistributionPageVM : BasePageVM
    {
        private int itemsPerPage;
        private int totalFilteredCount;
        private string search;
        private int lastPage;
        private DateTime startDate;
        private DateTime endDate;
        private ObservableCollection<SortParameter> sortParameters;
        private SortParameter selectedSort;
        private Paginator paginator;
        private Order selectedOrder;
        private Visibility emptyVisibility;

        public ProductDistributionPageVM()
        {
            Initialize();
            LoadData();
            ClientService.Instance.HubConnection.On<string>("UpdateOrders", (deliveries) =>
            {
                GetOrdersWithFilter();
            });
        }
        public RelayCommand SortOrderChangedCommand { get; set; }
        public RelayCommand ChangePageCommand { get; set; }
        public Paginator Paginator { get => paginator; set { paginator = value; OnPropertyChanged(); } }
        public ObservableCollection<Order> DisplayedOrders => Orders;
        public ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<Order> Orders { get; set; }
        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; OnPropertyChanged(); }
        }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        public string Search { get => search; set { search = value; OnPropertyChanged(); GetOrdersWithFilter(); } }
        public SortParameter SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(); GetOrdersWithFilter(); } }
        public Order SelectedOrder { get => selectedOrder; set { selectedOrder = value; OnPropertyChanged(); } }
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (value.Date != startDate.Date && value.Date <= EndDate.Date)
                {
                    startDate = value;
                    OnPropertyChanged();
                    GetOrdersWithFilter();
                }
            }
        }


        public DateTime EndDate
        {
            get => endDate;
            set
            {
                if (value.Date != endDate.Date && value.Date >= StartDate.Date)
                {
                    endDate = value;
                    OnPropertyChanged();
                    GetOrdersWithFilter();
                }
            }
        }
        private void Initialize()
        {
            ChangePageCommand = new RelayCommand(ChangePage);
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            // OpenProductWindowCommand = new RelayCommand(OpenProductWindow);
            // OpenEditProductWindowCommand = new RelayCommand(OpenEditProductWindow);
            // IsCategorySelected = false;
            search = string.Empty;
            ItemsPerPage = 15;
            lastPage = 1;
            startDate = DateTime.Now.ToLocalTime().AddMonths(-2);
            endDate = DateTime.Now.ToLocalTime().AddDays(7);
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Номер заказа", "OrderNumber"),
                new SortParameter("Дата заказа", "DateOfRegistration")
            };

            selectedSort = SortParameters.FirstOrDefault();

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedSort));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }


        private async void LoadData()
        {
            await LoadOrders();
        }
        private async Task LoadOrders()
        {
            await GetOrdersWithFilter();
            //OnPropertyChanged(nameof(DisplayedElectronics));
        }

        private async Task GetOrdersWithFilter()
        {
            var request = await ApiService.GetRequestWithParameter("api/Orders/Filter", "jsonFilter", JsonConvert.SerializeObject(
                new
                {
                    search = Search,
                    statusId = 2,
                    startDate = StartDate,
                    endDate = EndDate,
                    sortParameter = SelectedSort.Property,
                    isAscending = SelectedSort.IsAcsending,
                    currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
                    itemsPerPage = ItemsPerPage
                }));
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<FilteredOrders>(request.Content);
                Orders = new ObservableCollection<Order>(result.Orders);
                totalFilteredCount = result.TotalFilteredCount;
                if (Paginator != null)
                {
                    await RefreshOrders();
                }
                else
                {
                    Paginator = new Paginator(5, MaxPage());
                    await RefreshOrders();
                }
                //dd.Add(JsonConvert.DeserializeObject<List<Electronic>>(request.Content));
            }
        }

        private async Task RefreshOrders()
        {
            await Task.Run(() =>
            {
                var maxPage = MaxPage();

                Paginator.RefreshPages(maxPage == 0 ? 1 : maxPage);

                // var electronicsList = GetFilteredElectronics(Electronics);

                //Если после фильтрации у нас количество элементов 0, то выводим Пусто
                if (Orders.Count() <= 0)
                {
                    EmptyVisibility = Visibility.Visible;
                    Paginator.SelectedPageNumber = 1;
                }
                else EmptyVisibility = Visibility.Hidden;

                //electronicsList = electronicsList.Skip((Paginator.SelectedPageNumber - 1) * itemsPerPage)
                //   .Take(itemsPerPage).ToList();

                OnPropertyChanged(nameof(DisplayedOrders));
            });
        }
        private int MaxPage()
        {
            //Фильтруем наш список по поисковой строке
            //var list = GetFilteredElectronics(Electronics);

            return (int)Math.Ceiling((float)totalFilteredCount / (float)ItemsPerPage);
        }
        private async void SortOrderChanged(object obj)
        {
            //
            await GetOrdersWithFilter();
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
                        Paginator.ChangePage(MaxPage());
                    }
                }
                else return;
            }
            if (Paginator.DisplayedPagesNumbers.Count > 0)
            {
                await Task.Run(Paginator.RefrashPaginator);
                OnPropertyChanged(nameof(DisplayedOrders));
                if (lastPage != Paginator.SelectedPageNumber)
                {
                    lastPage = Paginator.SelectedPageNumber;
                    await GetOrdersWithFilter();
                }
                lastPage = Paginator.SelectedPageNumber;
            }
        }

    }
}
