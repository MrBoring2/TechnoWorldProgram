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
using TechnoWorld_Cash.Services;
using TechnoWorld_Cash.Views.Windows;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Services;
using TechnoWorld_WarehouseAccounting.Models;
using TechoWorld_DataModels_v2;
namespace TechnoWorld_Cash.ViewModels.Windows
{
    public class CashWindowViewModel : BaseWindowVM
    {
        private ObservableCollection<Order> displayedOrders;
        private List<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;
        private ObservableCollection<ItemWithTitle<Status>> statuses;
        private string search;
        private int currentPage;
        private int itemsPerPage;
        private int maxDisplayedPages;
        private int totalFilteredCount;
        private int totalPages;
        private int lastPage;
        private int selectedPageNumber;
        private Order selectedOrder;
        private ItemWithTitle<Status> selectedStatus;
        private DateTime startDate;
        private DateTime endDate;
        private Paginator paginator;
        private Visibility emptyVisibility;
        public CashWindowViewModel()
        {
            Initiailze();
            LoadData();
        }
        public ObservableCollection<Order> Orders { get; set; }
        public ObservableCollection<int> DisplayedPagesNumbers { get => displayedPagesNumbers; set { displayedPagesNumbers = value; OnPropertyChanged(); } }
        public List<int> PagesNumbers { get => pagesNumbers; set { pagesNumbers = value; OnPropertyChanged(); } }
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; OnPropertyChanged(); }
        }

        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; OnPropertyChanged(); }
        }
        public int MaxDisplayedPages
        {
            get { return maxDisplayedPages; }
            set { maxDisplayedPages = value; OnPropertyChanged(); }
        }
        public int TotalPages
        {
            get { return totalPages; }
            set { totalPages = value; OnPropertyChanged(); }
        }
        public int SelectedPageNumber
        {
            get { return selectedPageNumber; }
            set
            {
                selectedPageNumber = value;

                OnPropertyChanged();
            }
        }
        public ObservableCollection<ItemWithTitle<Status>> Statuses { get => statuses; set { statuses = value; OnPropertyChanged(); } }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        public ItemWithTitle<Status> SelectedStatus { get { return selectedStatus; } set { selectedStatus = value; OnPropertyChanged(); RefreshOrders(); } }
        public Order SelectedOrder { get { return selectedOrder; } set { selectedOrder = value; OnPropertyChanged(); } }
        public string Search { get { return search; } set { search = value; OnPropertyChanged(); RefreshOrders(); } }
        public DateTime StartDate
        {
            get => startDate;
            set { startDate = (value.Date <= endDate.Date && value.Date <= DateTime.Now.Date) || endDate == DateTime.MinValue ? value : startDate; OnPropertyChanged(); RefreshOrders(); }
        }
        public DateTime EndDate { get => endDate; set { endDate = (value.Date >= startDate.Date && value.Date <= DateTime.Now.Date) || startDate == DateTime.MinValue ? value : endDate; OnPropertyChanged(); RefreshOrders(); } }
        public ObservableCollection<Order> DisplayedOrders { get => displayedOrders; set { displayedOrders = value; OnPropertyChanged(); } }
        public Paginator Paginator { get => paginator; set { paginator = value; OnPropertyChanged(); } }
        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand ToFirstPageCommand { get; set; }
        public RelayCommand ToLastPageCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand CancelOrderCommand { get; set; }
        public RelayCommand PaymentCommand { get; set; }
        private void Initiailze()
        {

            search = string.Empty;
            ItemsPerPage = 15;
            lastPage = 1;
            startDate = DateTime.Now.ToLocalTime().AddMonths(-2);
            endDate = DateTime.Now.ToLocalTime().AddDays(7);
            ChangePageCommand = new RelayCommand(ChangePage);
            ExitCommand = new RelayCommand(Exit);
            PaymentCommand = new RelayCommand(Payment);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            EmptyVisibility = Visibility.Collapsed;
            ClientService.Instance.HubConnection.On<string>("UpdateOrders", (orders) =>
            {
                GetOrdersWithFilter();
            });
        }


        private async void Payment(object obj)
        {
            if (SelectedOrder.StatusId == 1)
            {
                var paymentVM = new PaymentWindowViewModel(SelectedOrder);
                await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(paymentVM));
            }
            else
            {
                CustomMessageBox.Show("Оплатить можно только заказы, которые ещё не оплачены.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }
        private void CancelOrder(object obj)
        {
            if (SelectedOrder.StatusId == 1)
            {
                SelectedOrder.StatusId = 4;
                SelectedOrder.EmployeeId = ClientService.Instance.User.UserId;
                var response = ApiService.PutRequest("api/Orders", SelectedOrder.OrderId, SelectedOrder);
            }
            else
            {
                CustomMessageBox.Show("Отменить можно только заказы, которые ещё не оплачены.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void LoadData()
        {
            await LoadStatuses();
            await LoadOrders();
            OnPropertyChanged(nameof(SelectedStatus));
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
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
                    sortParameter = "DateOfRegistration",
                    isAscending = true,
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
        private async Task LoadStatuses()
        {
            var statusesJson = await ApiService.GetRequest("api/Status");
            if (statusesJson.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Statuses = new ObservableCollection<ItemWithTitle<Status>>(JsonConvert.DeserializeObject<List<Status>>(statusesJson.Content).Select(p => new ItemWithTitle<Status>(p, p.Name)));
                Statuses = new ObservableCollection<ItemWithTitle<Status>>(Statuses.Where(p => p.Item.Id != 5 && p.Item.Id != 3));
                Statuses.Insert(0, new ItemWithTitle<Status>(null, "Все"));
                selectedStatus = Statuses.FirstOrDefault();
            }
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
        private void Exit(object obj)
        {
            ClientService.Instance.Logout();
            WindowNavigation.Instance.OpenAndHideWindow(this, new LoginWindowViewModel());
        }


    }
}
