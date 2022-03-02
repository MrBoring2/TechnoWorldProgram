using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Cash.Services;
using TechnoWorld_Cash.Views.Windows;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels;
namespace TechnoWorld_Cash.ViewModels.Windows
{
    public class CashWindowViewModel : BaseWindowVM
    {
        private ObservableCollection<Order> displayedOrders;
        private List<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;
        private string search;
        private int currentPage;
        private int itemsPerPage;
        private int maxDisplayedPages;
        private int totalPages;
        private int selectedPageNumber;
        private Order selectedOrder;
        private string selectedStatus;
        private DateTime startDate;
        private DateTime endDate;
        private Visibility emptyVisibility;
        public CashWindowViewModel()
        {
            Initiailze();
            LoadData();
        }
        public List<Order> Orders { get; set; }
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
        public List<string> Statuses { get; set; }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        public string SelectedStatus { get { return selectedStatus; } set { selectedStatus = value; OnPropertyChanged(); RefreshOrders(); } }
        public Order SelectedOrder { get { return selectedOrder; } set { selectedOrder = value; OnPropertyChanged(); } }
        public string Search { get { return search; } set { search = value; OnPropertyChanged(); RefreshOrders(); } }
        public DateTime StartDate
        {
            get => startDate;
            set { startDate = (value.Date <= endDate.Date && value.Date <= DateTime.Now.Date) || endDate == DateTime.MinValue ? value : startDate; OnPropertyChanged(); RefreshOrders(); }
        }
        public DateTime EndDate { get => endDate; set { endDate = (value.Date >= startDate.Date && value.Date <= DateTime.Now.Date) || startDate == DateTime.MinValue ? value : endDate; OnPropertyChanged(); RefreshOrders(); } }
        public ObservableCollection<Order> DisplayedOrders { get => displayedOrders; set { displayedOrders = value; OnPropertyChanged(); } }
        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand ToFirstPageCommand { get; set; }
        public RelayCommand ToLastPageCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand CancelOrderCommand { get; set; }
        public RelayCommand PaymentCommand { get; set; }
        private void Initiailze()
        {
            startDate = DateTime.Now.Date - new TimeSpan(7, 0, 0, 0);
            endDate = DateTime.Now;
            currentPage = 0;
            itemsPerPage = 20;
            maxDisplayedPages = 5;
            search = string.Empty;
            ChangePageCommand = new RelayCommand(ChangePage);
            ToFirstPageCommand = new RelayCommand(ToFirstPage);
            ToLastPageCommand = new RelayCommand(ToLastPage);
            ExitCommand = new RelayCommand(Exit);
            PaymentCommand = new RelayCommand(Payment);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            EmptyVisibility = Visibility.Collapsed;
            ClientService.Instance.HubConnection.On<string>("UpdateOrders", (orders) =>
            {
                Orders = JsonConvert.DeserializeObject<List<Order>>(orders);
                RefreshOrders();
            });
        }


        private async void Payment(object obj)
        {
            if (SelectedOrder.StatusId == 1)
            {
                var paymentVM = new PaymentWindowViewModel(SelectedOrder);
                await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(paymentVM));
                if (paymentVM.DialogResult == true)
                {
                    SelectedOrder.StatusId = 2;
                    SelectedOrder.EmployeeId = ClientService.Instance.User.UserId;
                    var response = ApiService.PutRequest("api/Orders", SelectedOrder.OrderId, SelectedOrder);

                }
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
            await Task.Run(LoadStatuses);
            await Task.Run(LoadOrders);
            OnPropertyChanged(nameof(SelectedStatus));
            OnPropertyChanged(nameof(CurrentPage));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private void LoadOrders()
        {
            var orders = ApiService.GetRequest("api/Orders");
            if (orders.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Orders = JsonConvert.DeserializeObject<List<Order>>(orders.Result.Content);
                RefreshOrders();

                LoadPages();
                DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers.Take(maxDisplayedPages));

                selectedPageNumber = DisplayedPagesNumbers.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedPageNumber));
            }
        }


        private void LoadStatuses()
        {
            Statuses = new List<string> { "Все" };
            var statusesJson = ApiService.GetRequest("api/Status");
            if (statusesJson.Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var statusesList = JsonConvert.DeserializeObject<List<Status>>(statusesJson.Result.Content);
                foreach (var item in statusesList)
                {
                    Statuses.Add(item.Name);
                }
                selectedStatus = Statuses.FirstOrDefault();
            }
        }

        private void RefreshOrders()
        {
            var list = Orders.Where(p => p.DateOfRegistration.Date <= EndDate.Date && p.DateOfRegistration.Date >= StartDate.Date).OrderBy(p => p.OrderNumber).ToList();
            list = list.Where(p => p.OrderNumber.ToLower().Contains(Search.ToLower())).OrderBy(p => p.OrderNumber).ToList();


            list = list.Where(p => SelectedStatus != "Все" ? p.Status.Name.Equals(SelectedStatus) : true).ToList();
            list = list.Skip((SelectedPageNumber - 1) * itemsPerPage)
                           .Take(itemsPerPage).ToList();
            DisplayedOrders = new ObservableCollection<Order>(list);

            if (DisplayedOrders.Count() <= 0)
            {
                EmptyVisibility = Visibility.Visible;
            }
            else EmptyVisibility = Visibility.Hidden;

        }
        private async void ToLastPage(object obj)
        {
            SelectedPageNumber = PagesNumbers.LastOrDefault();

            await Task.Run(RefrashPaginator);
            OnPropertyChanged(nameof(SelectedPageNumber));
        }

        private async void ToFirstPage(object obj)
        {
            SelectedPageNumber = PagesNumbers.FirstOrDefault();

            await Task.Run(RefrashPaginator);
            OnPropertyChanged(nameof(SelectedPageNumber));
        }
        private void ChangePage(object obj)
        {
            if (obj != null)
            {
                if (Convert.ToInt32(obj) == 0)
                {
                    SelectedPageNumber = 1;
                }
                else if (Convert.ToInt32(obj) == 1)
                {
                    SelectedPageNumber = MaxPage();
                }
            }
            if (DisplayedPagesNumbers.Count > 0)
            {
                RefrashPaginator();
                RefreshOrders();
            }
        }
        private async void LoadPages()
        {

            PagesNumbers = new List<int>();
            var max = MaxPage();
            for (int i = 0; i < max; i++)
            {
                PagesNumbers.Add(i + 1);
            }

        }
        private void Exit(object obj)
        {
            ClientService.Instance.Logout();
            WindowNavigation.Instance.OpenAndHideWindow(this, new LoginWindowViewModel());
        }
        private int MaxPage()
        {
            var list = Orders
                     .Where(p => p.OrderNumber.ToLower().Contains(Search.ToLower())).ToList();


            list = list.Where(p => SelectedStatus != "Все" ? p.Status.Name.Equals(SelectedStatus) : true).ToList();


            return (int)Math.Ceiling((float)list.Count / (float)ItemsPerPage);
        }
        public void RefrashPaginator()
        {
            if (SelectedPageNumber <= PageListAvg(DisplayedPagesNumbers))
            {
                DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                    .Take(maxDisplayedPages));
            }
            else
            {
                if (PagesNumbers.Skip(SelectedPageNumber - PageListAvg(DisplayedPagesNumbers)).Count() > maxDisplayedPages)
                    DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                        .Skip(SelectedPageNumber - PageListAvg(DisplayedPagesNumbers))
                        .Take(maxDisplayedPages));

                else
                    DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                       .Skip(PagesNumbers.Count - maxDisplayedPages)
                       .Take(maxDisplayedPages));
            }
        }
        private int PageListAvg(IEnumerable<int> collection)
        {
            return Convert.ToInt32(Math.Ceiling(collection.Count() / (float)2));
        }
    }
}
