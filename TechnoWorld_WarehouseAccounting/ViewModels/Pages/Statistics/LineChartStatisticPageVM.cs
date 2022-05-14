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
using TechnoWorld_WarehouseAccounting.Models.ForStatistics;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics
{
    public class LineChartStatisticPageVM : BaseSaleStatisticsPageVM
    {
        public LineChartStatisticPageVM(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
            CreateChart();
        }
        public async void CreateChart()
        {
            Sales = new ObservableCollection<SaleModel>();
            Labels = new ObservableCollection<string>();
            SeriesCollection = new SeriesCollection();

            var responseOrders = await ApiService.Instance.GetRequestWithParameter("api/Orders/ForStatistics", "chartParams", JsonConvert.SerializeObject(new
            {
                electronicsTypeId = 0,
                categoryId = 1,
                startDate = _startDate,
                endDate = _endDate
            }));

            var responseTypes = await ApiService.Instance.GetRequest("api/ElectrnicsTypes/All");
            var responseCategories = await ApiService.Instance.GetRequest("api/Categories");


            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                if (responseOrders.StatusCode == System.Net.HttpStatusCode.OK && responseTypes.StatusCode == System.Net.HttpStatusCode.OK &&
                    responseCategories.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var orders = JsonConvert.DeserializeObject<List<Order>>(responseOrders.Content);
                    var types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(responseTypes.Content);
                    var categories = JsonConvert.DeserializeObject<List<OrderElectronic>>(responseCategories.Content);
                    foreach (var type in types.OrderBy(p => p.Name))
                    {
                        int totalCount = 0;
                        decimal totalSales = 0;
                        ChartValues<SalesTooltip> values = new ChartValues<SalesTooltip>();
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

                            var list = new List<DateTimePoint>();
                            values.Add(new SalesTooltip(type.Name, count, sales, date.Ticks));
                        }
                        SeriesCollection.Add(new LineSeries
                        {
                            Title = type.Name,
                            Values = values,
                            FontSize = 14,
                            DataLabels = true,

                        });

                        Sales.Add(new SaleModel(type.Name, totalCount, totalSales));
                    }
                }

                var typesMapper = Mappers.Xy<SalesTooltip>()
                    .X((value, index) => value.Ticks)
                    .Y(value => value.Count);

                Charting.For<SalesTooltip>(typesMapper);

                XFormatter = p => new DateTime((long)p).ToString("d");
                OnPropertyChanged(nameof(SeriesCollection));
                OnPropertyChanged(nameof(TotalPriceForPeriod));
            });
        }
    }
}
