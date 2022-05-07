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
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class ProductDistributionPageVM : ListViewPageVM<Order, FilteredOrders>
    {
        private DateTime startDate;
        private DateTime endDate;
        private Order selectedOrder;
        private ObservableCollection<SortParameter> sortParameters;
        public ProductDistributionPageVM() : base(15)
        {
            Initialize();
            LoadData();
            ClientService.Instance.HubConnection.On<string>("UpdateOrders", (deliveries) =>
            {
                GetWithFilter();
            });
        }
        public RelayCommand OpenDestributionWindowCommand { get; set; }
        public override ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<Order> Orders { get; set; }
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

        protected override string UrlApi => "api/Orders/Filter";


        protected override object FilterParam => new
        {
            search = Search,
            statusId = 2,
            startDate = StartDate,
            endDate = EndDate,
            sortParameter = SelectedSort.Property,
            isAscending = SelectedSort.IsAcsending,
            currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
            itemsPerPage = ItemsPerPage
        };

        private void Initialize()
        {
            OpenDestributionWindowCommand = new RelayCommand(OpenDestributionWindow);
            startDate = DateTime.Now.ToLocalTime().AddMonths(-2);
            endDate = DateTime.Now.ToLocalTime().AddDays(7);
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Номер заказа", "OrderNumber"),
                new SortParameter("Дата заказа", "DateOfRegistration")
            };

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedSort));
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
        }

        private async void LoadData()
        {
            await GetWithFilter();
        }

        private async void OpenDestributionWindow(object obj)
        {
            var destributionWindow = new DestributionOrderWindowVM(SelectedOrder);
            await Task.Run(() => WindowNavigation.Instance.OpenModalWindow(destributionWindow));
            if (destributionWindow.DialogResult == true)
            {
                CustomMessageBox.Show($"Товары из заказа {SelectedOrder.OrderNumber} успешно выданы со склада.", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
