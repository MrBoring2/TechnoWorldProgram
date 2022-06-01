using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TechnoWorld_WarehouseAccounting.Models.ForStatistics;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics
{
    public class PieChartStatisticsPageVM : BaseSaleStatisticsPageVM
    {
        private string _statisticsType;
        private Visibility emptyVisibility;
        private List<Order> orders;
        private List<ElectrnicsType> types;
        private List<Category> categories;
        public PieChartStatisticsPageVM(string statisticsType, DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
            EmptyVisibility = Visibility.Collapsed;
            _statisticsType = statisticsType;
            PointLabel = chartPoint =>
                string.Format("{0} шт. ({1:P})", chartPoint.Y, chartPoint.Participation);
            var typesMapper = Mappers.Pie<PieSalesTooltip>()
               .Value(value => value.Count);

            Charting.For<PieSalesTooltip>(typesMapper);
            LoadData();
        }
        public Func<ChartPoint, string> PointLabel { get; set; }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        private async void LoadData()
        {
            if (_statisticsType == "Продажи по типам товаров")
            {
                await LoadTypes();
            }
            else if (_statisticsType == "Продажи по категориям")
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
            if (responseOrders.StatusCode == System.Net.HttpStatusCode.OK)
            {
                orders = JsonConvert.DeserializeObject<List<Order>>(responseOrders.Content);

                CreateChart();
            }
        }

        private async Task LoadCategories()
        {
            var responseCategories = await ApiService.Instance.GetRequest("api/Categories");
            if (responseCategories.StatusCode == System.Net.HttpStatusCode.OK)
            {
                categories = JsonConvert.DeserializeObject<List<Category>>(responseCategories.Content);
            }
        }

        private async Task LoadTypes()
        {
            var responseTypes = await ApiService.Instance.GetRequest("api/ElectrnicsTypes/All");
            if (responseTypes.StatusCode == System.Net.HttpStatusCode.OK)
            {
                types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(responseTypes.Content);
            }
        }
        public async void CreateChart()
        {
            Sales = new ObservableCollection<SaleModel>();
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();
            if (SeriesCollection == null)
            {
                SeriesCollection = new SeriesCollection();
            }
            else
            {
                SeriesCollection.Clear();
            }
            if (orders.Count == 0)
            {
                EmptyVisibility = Visibility.Visible;
                return;
            }
            else
            {
                EmptyVisibility = Visibility.Collapsed; 
                return;
            }
            if (_statisticsType == "Продажи по типам товаров")
            {
                foreach (var type in types.OrderBy(p => p.Name))
                {
                    int totalCount = 0;
                    decimal totalSales = 0;
                    //ChartValues<double> values = new ChartValues<double>();
                    ChartValues<PieSalesTooltip> values = new ChartValues<PieSalesTooltip>();
                    for (DateTime date = _startDate; date <= _endDate; date = date.AddDays(1))
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


                    }
                    if (totalCount > 0)
                    {
                        values.Add(new PieSalesTooltip(type.Name, totalCount, totalSales));
                    }
                    //values.Add(new SalesTooltip(type.Name, totalCount, totalSales, date.Ticks));
                    SeriesCollection.Add(new PieSeries
                    {
                        Title = type.Name,
                        Values = values,
                        DataLabels = true,
                        LabelPoint = PointLabel,
                        Stroke = Brushes.Black,
                        FontSize = 14,
                        LabelPosition = PieLabelPosition.OutsideSlice,

                        Foreground = Brushes.Black
                    });

                    if ((values as ChartValues<PieSalesTooltip>).All(d => d.Count == 0))
                    {
                        EmptyVisibility = Visibility.Visible;
                    }
                    else
                    {
                        EmptyVisibility = Visibility.Collapsed;
                    }

                    Labels.Add(type.Name);
                    Sales.Add(new SaleModel(type.Name, totalCount, totalSales));
                }
            }
            else if (_statisticsType == "Продажи по категориям")
            {
                foreach (var category in categories.OrderBy(p => p.Name))
                {
                    int totalCount = 0;
                    decimal totalSales = 0;
                    //ChartValues<double> values = new ChartValues<double>();
                    ChartValues<PieSalesTooltip> values = new ChartValues<PieSalesTooltip>();
                    for (DateTime date = _startDate; date <= _endDate; date = date.AddDays(1))
                    {
                        decimal sales = 0;
                        int count = 0;
                        foreach (var order in orders.Where(p => p.DateOfRegistration.Date == date.Date))
                        {
                            foreach (var orderElectronics in order.OrderElectronics)
                            {
                                if (orderElectronics.Electronics.Type.CategoryId == category.Id)
                                {
                                    count += orderElectronics.Count;
                                    sales += orderElectronics.Electronics.SalePrice * orderElectronics.Count;
                                }
                            }
                        }

                        totalCount += count;
                        totalSales += sales;


                    }
                    if (totalCount > 0)
                    {
                        values.Add(new PieSalesTooltip(category.Name, totalCount, totalSales));
                    }
                    //values.Add(new SalesTooltip(type.Name, totalCount, totalSales, date.Ticks));
                    SeriesCollection.Add(new PieSeries
                    {
                        Title = category.Name,
                        Values = values,
                        DataLabels = true,
                        LabelPoint = PointLabel,
                        Stroke = Brushes.Black,
                        FontSize = 14,
                        LabelPosition = PieLabelPosition.OutsideSlice,

                        Foreground = Brushes.Black
                    });



                    Labels.Add(category.Name);
                    Sales.Add(new SaleModel(category.Name, totalCount, totalSales));
                }
                if (SeriesCollection.All(p => (p.Values as ChartValues<PieSalesTooltip>).All(d => d.Count == 0)))
                {
                    EmptyVisibility = Visibility.Visible;
                }
                else
                {
                    EmptyVisibility = Visibility.Collapsed;
                }
            }





            XFormatter = p => new DateTime((long)p).ToString("d");
            OnPropertyChanged(nameof(SeriesCollection));
            OnPropertyChanged(nameof(TotalPriceForPeriod));
        }

    }
}
