using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_API.Models;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class DeliveryManagementPageVM : BasePageVM
    {
        private int itemsPerPage;
        private Paginator paginator;
        private Visibility emptyVisibility;
        private string search;
        private int lastPage;
        private int totalFilteredCount;
        private ObservableCollection<Delivery> deliveries;
        private ObservableCollection<ItemWithTitle<Status>> statuses;
        private ObservableCollection<SortParameter> sortParameters;
        private SortParameter selectedSort;
        private ItemWithTitle<Status> selectedStatus;
        private Delivery selectedDelivery;
        private DateTime startDate;
        private DateTime endDate;
        public DeliveryManagementPageVM()
        {
            Initialize();
            LoadData();
            ClientService.Instance.HubConnection.On<string>("UpdateDeliveries", (deliveries) =>
            {
                GetDeliveriesWithFilter();
            });
        }
        public Paginator Paginator { get => paginator; set { paginator = value; OnPropertyChanged(); } }
        public RelayCommand OpenDeliveryWindowCommand { get; set; }
        public RelayCommand SortOrderChangedCommand { get; set; }
        public RelayCommand OpenEditDeliveryWindowCommand { get; set; }
        public RelayCommand ChangePageCommand { get; set; }
        public ObservableCollection<Delivery> Deliveries { get; set; }
        public ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<Status>> Statuses { get => statuses; set { statuses = value; OnPropertyChanged(); } }
        public ObservableCollection<Delivery> DisplayedDeliveries => Deliveries;
        public Delivery SelectedDelivery { get => selectedDelivery; set { selectedDelivery = value; OnPropertyChanged(); } }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        public string Search
        {
            get => search;
            set { search = value; OnPropertyChanged(); GetDeliveriesWithFilter(); }
        }
        public ItemWithTitle<Status> SelectedStatus { get => selectedStatus; set { selectedStatus = value; OnPropertyChanged(); GetDeliveriesWithFilter(); } }
        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; OnPropertyChanged(); }
        }
        public SortParameter SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(); GetDeliveriesWithFilter(); } }
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (value.Date != startDate.Date && value.Date <= EndDate.Date)
                {
                    startDate = value;
                    OnPropertyChanged();
                    GetDeliveriesWithFilter();
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
                    GetDeliveriesWithFilter();
                }
            }
        }
        private void Initialize()
        {

            OpenDeliveryWindowCommand = new RelayCommand(OpenDeliveryWindow);
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            OpenEditDeliveryWindowCommand = new RelayCommand(OpenEditDeliveryWindow);
            ChangePageCommand = new RelayCommand(ChangePage);
            search = string.Empty;
            ItemsPerPage = 15;
            startDate = DateTime.Now.ToLocalTime().AddDays(-7);
            endDate = DateTime.Now.ToLocalTime().AddDays(7);
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Номер", "DeliveryNumber"),
                new SortParameter("Дата заказа", "DateOfOrder"),
                new SortParameter("Дата поставки", "DateOfDelivery"),
            };
            selectedSort = SortParameters.FirstOrDefault();

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedSort));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }


        private async void LoadData()
        {
            await LoadStatuses();
            await LoadDeliveries();
        }

        private Task LoadStatuses()
        {
            return Task.Run(async () =>
            {
                var response = await ApiService.GetRequest("api/Status");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Statuses = new ObservableCollection<ItemWithTitle<Status>>(JsonConvert.DeserializeObject<List<Status>>(response.Content).Select(p => new ItemWithTitle<Status>(p, p.Name)));
                    Statuses.Insert(0, new ItemWithTitle<Status>(null, "Все"));
                    Statuses.Remove(Statuses.FirstOrDefault(p => p.Title.Equals("Ожидается вадыча")));
                    selectedStatus = Statuses.FirstOrDefault();
                    OnPropertyChanged(nameof(SelectedStatus));
                }
            });
        }

        private async Task LoadDeliveries()
        {
            await GetDeliveriesWithFilter();
        }

        private async Task GetDeliveriesWithFilter()
        {
            var request = await ApiService.GetRequestWithParameter("api/Deliveries/Filter", "jsonFilter", JsonConvert.SerializeObject(
                new
                {
                    search = Search,
                    startDate = StartDate,
                    endDate = EndDate,
                    statusId = SelectedStatus.Item == null ? 0 : SelectedStatus.Item.Id,
                    sortParameter = SelectedSort.Property,
                    isAscending = SelectedSort.IsAcsending,
                    currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
                    itemsPerPage = ItemsPerPage
                }));
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<FilteredDeliveries>(request.Content);
                Deliveries = new ObservableCollection<Delivery>(result.Deliveries);
                totalFilteredCount = result.TotalFilteredCount;
                if (Paginator != null)
                {
                    await RefreshDelieries();
                }
                else
                {
                    Paginator = new Paginator(5, MaxPage());
                    await RefreshDelieries();
                }
                //dd.Add(JsonConvert.DeserializeObject<List<Electronic>>(request.Content));
            }

        }


        private async void SortOrderChanged(object obj)
        {
            await GetDeliveriesWithFilter();
        }
        private async void OpenDeliveryWindow(object obj)
        {
            var deliveryWindowVM = new DeliveryWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(deliveryWindowVM));

            if (deliveryWindowVM.DialogResult == true)
            {
                CustomMessageBox.Show($"Заказ поставщику номер {deliveryWindowVM.Delivery.DeliveryNumber} упешно добавлен, сформирована приходаня накладная!", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private async void OpenEditDeliveryWindow(object obj)
        {
            if (SelectedDelivery != null)
            {
                var deliveryWindowVM = new DeliveryWindowVM(SelectedDelivery);
                await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(deliveryWindowVM));
            }           
        }

        private async Task RefreshDelieries()
        {
            await Task.Run(() =>
            {
                var maxPage = MaxPage();

                Paginator.RefreshPages(maxPage == 0 ? 1 : maxPage);

                //Если после фильтрации у нас количество элементов 0, то выводим Пусто
                if (Deliveries.Count() <= 0)
                {
                    EmptyVisibility = Visibility.Visible;
                    Paginator.SelectedPageNumber = 1;
                }
                else EmptyVisibility = Visibility.Hidden;

                OnPropertyChanged(nameof(DisplayedDeliveries));
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
                        Paginator.ChangePage(MaxPage());
                    }
                }
                else return;
            }
            if (Paginator.DisplayedPagesNumbers.Count > 0)
            {
                await Task.Run(Paginator.RefrashPaginator);
                OnPropertyChanged(nameof(DisplayedDeliveries));
                if (lastPage != Paginator.SelectedPageNumber)
                {
                    lastPage = Paginator.SelectedPageNumber;
                    await GetDeliveriesWithFilter();
                }
                lastPage = Paginator.SelectedPageNumber;
            }
        }
    }
}
