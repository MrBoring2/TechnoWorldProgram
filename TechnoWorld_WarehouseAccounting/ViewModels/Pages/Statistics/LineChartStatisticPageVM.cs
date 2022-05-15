using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Models.ForStatistics;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics
{
    public class LineChartStatisticPageVM : BaseSaleStatisticsPageVM
    {
        private int dayInterval;
        private string _statisticsType;
        private int minY;
        private int maxY;
        private List<Order> orders;
        private List<ElectrnicsType> types;
        private List<Category> categories;
        public LineChartStatisticPageVM(string statisticsType, DateTime startDate, DateTime endDate)
        {
            dayInterval = 30;
            maxY = 1;
            _startDate = startDate;
            _endDate = endDate;
            From = _endDate.Day >= DateTime.DaysInMonth(_endDate.Year, _endDate.Month) ? _endDate.AddMonths(-1).Ticks : new DateTime(_endDate.Year, _endDate.Month, 1).Ticks;
            To = _endDate.Ticks;
            _statisticsType = statisticsType;
            LoadData();
            OnPropertyChanged(nameof(CurrentPeriod));
            NextPeriodCommand = new RelayCommand(NextPeriod);
            BackPeriodCommand = new RelayCommand(BackPeriod);
            var typesMapper = Mappers.Xy<SalesTooltip>()
                   .X((value, index) => value.Ticks)
                   .Y(value => value.Count);

            Charting.For<SalesTooltip>(typesMapper);

        }
        public RelayCommand BackPeriodCommand { get; set; }
        public RelayCommand NextPeriodCommand { get; set; }

        public string CurrentPeriod => $"{new DateTime(From).ToShortDateString()} - {new DateTime(To).ToShortDateString()}";
        private async void LoadData()
        {
           
            if (_statisticsType == "Продажи по типам товаров")
            {
                await LoadTypes();
            }
            else if(_statisticsType == "Продажи по категориям")
            {
                await LoadCategories();
            }
           
            await LoadOrders();
        }

        private async Task LoadOrders()
        {
            var responseOrders = await ApiService.Instance.GetRequestWithParameter("api/Orders/ForStatistics", "chartParams", JsonConvert.SerializeObject(new
            {
                startDate = _startDate,
                endDate = _endDate
            }));
            if(responseOrders.StatusCode == System.Net.HttpStatusCode.OK)
            {
                orders = JsonConvert.DeserializeObject<List<Order>>(responseOrders.Content);
            
                CreateChart();
            }
        }

        private async Task LoadCategories()
        {
            var responseCategories = await ApiService.Instance.GetRequest("api/Categories");
            if(responseCategories.StatusCode == System.Net.HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(responseCategories.Content);
            }
        }

        private async Task LoadTypes()
        {
            var responseTypes = await ApiService.Instance.GetRequest("api/ElectrnicsTypes/All");
            if(responseTypes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(responseTypes.Content);
            }
        }

        private void BackPeriod(object obj)
        {
            if (new DateTime(From) > _startDate)
            {

                var a = new DateTime(From).AddMonths(-1);
                if ((new DateTime(From).AddMonths(-1)) < _startDate)
                {
                    var date = new DateTime(To).AddMonths(-1);
                    From = _startDate.Ticks;
                    To = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month)).Ticks;
                }
                else
                {
                    var date = new DateTime(To);

                    From -= (new DateTime(From) - new DateTime(From).AddMonths(-1)).Ticks;
                    var fromdate = new DateTime(From);
                    To = new DateTime(fromdate.Year, fromdate.Month, DateTime.DaysInMonth(fromdate.Year, fromdate.Month)).Ticks;
                }
                OnPropertyChanged(nameof(CurrentPeriod));
                CreateChart();
            }

        }


        private void NextPeriod(object obj)
        {
            if (new DateTime(To) < _endDate)
            {
                if ((new DateTime(To).AddMonths(1) > _endDate))
                {
                    From = new DateTime(_endDate.Year, _endDate.Month, 1).Ticks;
                    To = _endDate.Ticks;
                }
                else
                {
                    var date = new DateTime(From).AddMonths(1);
                    From = new DateTime(date.Year, date.Month, 1).Ticks;
                    var fromdate = new DateTime(From);
                    To = new DateTime(fromdate.Year, fromdate.Month, DateTime.DaysInMonth(fromdate.Year, fromdate.Month)).Ticks;
                }
                OnPropertyChanged(nameof(CurrentPeriod));
                CreateChart();
            }
        }
        public int MinY { get => minY; set { minY = value; OnPropertyChanged(); } }
        public int MaxY { get => maxY; set { maxY = value; OnPropertyChanged(); } }
        public async void CreateChart()
        {
            Sales = new ObservableCollection<SaleModel>();
            Labels = new ObservableCollection<string>();
            if (SeriesCollection == null)
            {
                SeriesCollection = new SeriesCollection();
            }
            else
            {
                SeriesCollection.Clear();
            }

            if (_statisticsType == "Продажи по типам товаров")
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var type in types.OrderBy(p => p.Name))
                    {
                        int totalCount = 0;
                        decimal totalSales = 0;
                        ChartValues<SalesTooltip> values = new ChartValues<SalesTooltip>();
                        SalesTooltip[] valuesMass = new SalesTooltip[(new DateTime(To) - new DateTime(From)).Days + 1];
                        int i = 0;
                        for (DateTime date = new DateTime(From); date <= new DateTime(To); date = date.AddDays(1))
                        {
                            decimal sales = 0;
                            int count = 0;
                            foreach (var order in orders.Where(p => p.DateOfRegistration.Date == date.Date))
                            {
                                foreach (var orderElectronics in order.OrderElectronics)
                                {
                                    if (orderElectronics.Electronics.TypeId == type.TypeId)
                                    {
                                        count += orderElectronics.Count;
                                        sales += orderElectronics.Electronics.SalePrice * orderElectronics.Count;
                                    }
                                }
                            }
                            totalCount += count;
                            totalSales += sales;

                            if (totalCount > MaxY)
                            {
                                MaxY = totalCount;
                            }

                            var list = new List<DateTimePoint>();
                            valuesMass[i] = (new SalesTooltip(type.Name, count, sales, date.Ticks));
                            i++;
                        }
                        values.AddRange(valuesMass);
                        SeriesCollection.Add(new LineSeries
                        {
                            Title = type.Name,
                            Values = values,
                            FontSize = 14,
                        });
                        Sales.Add(new SaleModel(type.Name, totalCount, totalSales));
                    }
                });
            }
            else if (_statisticsType == "Продажи по категориям")
            {
                await App.Current.Dispatcher.InvokeAsync(() =>
                {
                    foreach (var category in categories.OrderBy(p => p.Name))
                    {
                        int totalCount = 0;
                        decimal totalSales = 0;
                        ChartValues<SalesTooltip> values = new ChartValues<SalesTooltip>();
                        SalesTooltip[] valuesMass = new SalesTooltip[(new DateTime(To) - new DateTime(From)).Days + 1];
                        int i = 0;
                        for (DateTime date = new DateTime(From); date <= new DateTime(To); date = date.AddDays(1))
                        {
                            decimal sales = 0;
                            int count = 0;
                            foreach (var order in orders.Where(p => p.DateOfRegistration.Date == date.Date))
                            {
                                foreach (var orderElectronics in order.OrderElectronics)
                                {
                                    if (orderElectronics.Electronics.TypeId == category.Id)
                                    {
                                        count += orderElectronics.Count;
                                        sales += orderElectronics.Electronics.SalePrice * orderElectronics.Count;
                                    }
                                }
                            }
                            totalCount += count;
                            totalSales += sales;

                            if (totalCount > MaxY)
                            {
                                MaxY = totalCount;
                            }

                            var list = new List<DateTimePoint>();
                            valuesMass[i] = (new SalesTooltip(category.Name, count, sales, date.Ticks));
                            i++;
                        }
                        values.AddRange(valuesMass);
                        SeriesCollection.Add(new LineSeries
                        {
                            Title = category.Name,
                            Values = values,
                            FontSize = 14,
                        });
                        Sales.Add(new SaleModel(category.Name, totalCount, totalSales));
                    }
                });
            }



            XFormatter = p => new DateTime((long)p).ToString("d");
            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(TotalPriceForPeriod));
        }
    }
}

