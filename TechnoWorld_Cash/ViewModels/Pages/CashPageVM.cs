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
using TechnoWorld_Cash.ViewModels.Windows;
using TechnoWorld_Cash.Views.Windows;
using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_Helpers.Services;
using WPF_VM_Abstractions;

namespace TechnoWorld_Cash.ViewModels.Pages
{
    public class CashPageVM : ListEntitiesPageVM<Order, FilteredOrders>
    {
        private ObservableCollection<ItemWithTitle<OrderStatus>> statuses;
        private ItemWithTitle<OrderStatus> selectedStatus;
        private DateTime startDate;
        private DateTime endDate;

        public CashPageVM() : base(15)
        {
            Initiailze();
            LoadData();
        }
        public ObservableCollection<Order> Orders { get; set; }

        public ObservableCollection<ItemWithTitle<OrderStatus>> Statuses { get => statuses; set { statuses = value; OnPropertyChanged(); } }

        public ItemWithTitle<OrderStatus> SelectedStatus { get { return selectedStatus; } set { selectedStatus = value; OnPropertyChanged(); GetWithFilter(); } }

        public DateTime StartDate
        {
            get => startDate;
            set { startDate = (value.Date <= endDate.Date && value.Date <= DateTime.Now.Date) || endDate == DateTime.MinValue ? value : startDate; OnPropertyChanged(); GetWithFilter(); }
        }
        public DateTime EndDate { get => endDate; set { endDate = (value.Date >= startDate.Date && value.Date <= DateTime.Now.Date) || startDate == DateTime.MinValue ? value : endDate; OnPropertyChanged(); GetWithFilter(); } }
        public ObservableCollection<Order> DisplayedOrders => Orders;
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand CancelOrderCommand { get; set; }
        public RelayCommand PaymentCommand { get; set; }
        public RelayCommand RestoreOrderCommand { get; set; }

        protected override string UrlApi => "api/Orders/Filter";

        public override ObservableCollection<SortParameter> SortParameters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        protected override object FilterParam => new
        {
            search = Search,
            statusId = SelectedStatus.Item == null ? 0 : SelectedStatus.Item.Id,
            startDate = StartDate,
            endDate = EndDate,
            sortParameter = "DateOfRegistration",
            isAscending = false,
            currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
            itemsPerPage = ItemsPerPage
        };

        private void Initiailze()
        {
            startDate = DateTime.Now.ToLocalTime().AddMonths(-2);
            endDate = DateTime.Now.ToLocalTime().AddDays(7);
            PaymentCommand = new RelayCommand(Payment);
            CancelOrderCommand = new RelayCommand(CancelOrder);
            RestoreOrderCommand = new RelayCommand(RestoreOrder);
            EmptyVisibility = Visibility.Collapsed;
            ApiService.Instance.GetHubConnection.On<string>("UpdateOrders", async (orders) =>
            {
                await GetWithFilter();
            });
        }


        private async void Payment(object obj)
        {
            if (SelectedEntity != null)
            {
                if (SelectedEntity.StatusId == 1)
                {
                    var paymentVM = new PaymentWindowViewModel(SelectedEntity);
                    await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(paymentVM));
                }
                else
                {
                    MaterialNotification.Show("Внимание", $"Оплатить можно только заказ, требющий оплаты", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                }
            }
        }
        private async void CancelOrder(object obj)
        {
            if (SelectedEntity != null)
            {
                if (SelectedEntity.StatusId == 1)
                {
                    var result = MaterialNotification.Show("Подтверждение", $"Подтвердите отмену заказа.", MaterialNotificationButton.YesNo, MaterialNotificationImage.Question);
                    if (result == MaterialNotificationResult.Yes)
                    {

                        SelectedEntity.StatusId = 2;
                        SelectedEntity.EmployeeId = ClientService.Instance.User.UserId;
                        var response = await ApiService.Instance.PutRequest("api/Orders", SelectedEntity.OrderId, SelectedEntity);
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            MaterialNotification.Show("Произошла ошибка при изменении статуса товара", $"{response.Content}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                        }
                    }
                }
                else
                {
                    MaterialNotification.Show("Внимание", $"Отменить можно только заказ, который ещё не оплачен.", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                }
            }
        }
        private async void RestoreOrder(object obj)
        {
            if (SelectedEntity != null)
            {
                if (SelectedEntity.StatusId == 2)
                {
                    SelectedEntity.StatusId = 1;
                    SelectedEntity.EmployeeId = ClientService.Instance.User.UserId;
                    var response = await ApiService.Instance.PutRequest("api/Orders", SelectedEntity.OrderId, SelectedEntity);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        MaterialNotification.Show("Произошла ошибка при изменении статуса товара", $"{response.Content}", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }
                }
                else
                {
                    MaterialNotification.Show("Внимание", $"Восстановить можно только заказ, которые был отменён.", MaterialNotificationButton.Ok, MaterialNotificationImage.Warning);
                }
            }
        }

        private async void LoadData()
        {
            await LoadStatuses();
            await GetWithFilter();
            OnPropertyChanged(nameof(SelectedStatus));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private async Task LoadStatuses()
        {
            var statusesJson = await ApiService.Instance.GetRequest("api/OrderStatus");
            if (statusesJson.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Statuses = new ObservableCollection<ItemWithTitle<OrderStatus>>(JsonConvert.DeserializeObject<List<OrderStatus>>(statusesJson.Content).Select(p => new ItemWithTitle<OrderStatus>(p, p.Name)));
                Statuses.Remove(Statuses.FirstOrDefault(p => p.Item.Id == 2));
                Statuses.Remove(Statuses.FirstOrDefault(p => p.Item.Id == 4));
                Statuses.Insert(0, new ItemWithTitle<OrderStatus>(null, "Все"));
                selectedStatus = Statuses.FirstOrDefault();
            }
        }

    }
}
