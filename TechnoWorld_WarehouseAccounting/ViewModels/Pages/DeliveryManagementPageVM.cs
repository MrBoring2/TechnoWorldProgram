using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
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
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class DeliveryManagementPageVM : ListEntitiesPageVM<Delivery, FilteredDeliveries>
    {
        private ObservableCollection<ItemWithTitle<DeliveryStatus>> statuses;
        private ObservableCollection<SortParameter> sortParameters;
        private ItemWithTitle<DeliveryStatus> selectedStatus;
        private DateTime startDate;
        private DateTime endDate;
        public DeliveryManagementPageVM() : base(15)
        {
            Initialize();
            LoadData();
            ApiService.Instance.GetHubConnection.On<string>("UpdateDeliveries", async (deliveries) =>
            {
                await GetWithFilter();
            });
        }
        public RelayCommand OpenDeliveryWindowCommand { get; set; }
        public RelayCommand OpenEditDeliveryWindowCommand { get; set; }
        public override ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<DeliveryStatus>> Statuses { get => statuses; set { statuses = value; OnPropertyChanged(); } }
        public ItemWithTitle<DeliveryStatus> SelectedStatus { get => selectedStatus; set { selectedStatus = value; OnPropertyChanged(); GetWithFilter(); } }
        public DateTime StartDate
        {
            get => startDate;
            set
            {
                if (value.Date != startDate.Date && value.Date <= EndDate.Date)
                {
                    startDate = value;
                    OnPropertyChanged();
                    GetWithFilter();
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
                    GetWithFilter();
                }
            }
        }

        protected override string UrlApi => "api/Deliveries/Filter";
        protected override object FilterParam => new
        {
            search = Search,
            startDate = StartDate,
            endDate = EndDate,
            statusId = SelectedStatus.Item == null ? 0 : SelectedStatus.Item.Id,
            sortParameter = SelectedSort.Property,
            isAscending = SelectedSort.IsAcsending,
            currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
            itemsPerPage = ItemsPerPage
        };

        private void Initialize()
        {

            OpenDeliveryWindowCommand = new RelayCommand(OpenDeliveryWindow);
            OpenEditDeliveryWindowCommand = new RelayCommand(OpenEditDeliveryWindow);
            startDate = DateTime.Now.ToLocalTime().AddDays(-7);
            endDate = DateTime.Now.ToLocalTime().AddDays(7);
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Номер", "DeliveryNumber"),
                new SortParameter("Дата заказа", "DateOfOrder"),
                new SortParameter("Дата поставки", "DateOfDelivery"),
            };

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedSort));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }


        private async void LoadData()
        {
            await LoadStatuses();
            await GetWithFilter();
        }

        private Task LoadStatuses()
        {
            return Task.Run(async () =>
            {
                var response = await ApiService.Instance.GetRequest("api/DeliveryStatus");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Statuses = new ObservableCollection<ItemWithTitle<DeliveryStatus>>(JsonConvert.DeserializeObject<List<DeliveryStatus>>(response.Content).Select(p => new ItemWithTitle<DeliveryStatus>(p, p.Name)));
                    Statuses.Insert(0, new ItemWithTitle<DeliveryStatus>(null, "Все"));
                    selectedStatus = Statuses.FirstOrDefault();
                    OnPropertyChanged(nameof(SelectedStatus));
                }
            });
        }
        private async void OpenDeliveryWindow(object obj)
        {
            var deliveryWindowVM = new DeliveryWindowVM();
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(deliveryWindowVM));

            if (deliveryWindowVM.DialogResult == true)
            {
                MaterialNotification.Show("Оповещение", $"Заказ поставщику номер {deliveryWindowVM.Delivery.DeliveryNumber} упешно добавлен, сформирована приходаня накладная!", MaterialNotificationButton.Ok, MaterialNotificationImage.Susccess);
            }
        }
        private async void OpenEditDeliveryWindow(object obj)
        {
            if (SelectedEntity != null)
            {
                var deliveryWindowVM = new DeliveryWindowVM(SelectedEntity);
                await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(deliveryWindowVM));
            }
        }
    }
}
