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
using System.Windows.Media;
using TechnoWorld_WarehouseAccounting.Models.ForStatistics;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics
{
    public class PieChartStatisticsPageVM : BaseSaleStatisticsPageVM
    {
        public Func<ChartPoint, string> PointLabel { get; set; }
        public PieChartStatisticsPageVM(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
            PointLabel = chartPoint =>
                string.Format("{0} шт. ({1:P})", chartPoint.Y, chartPoint.Participation);
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
                        values.Add(new PieSalesTooltip(type.Name, totalCount, totalSales));
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
                        Labels.Add(type.Name);
                        Sales.Add(new SaleModel(type.Name, totalCount, totalSales));
                    }
                }



                var typesMapper = Mappers.Pie<PieSalesTooltip>()
                    .Value(value => value.Count);

                Charting.For<PieSalesTooltip>(typesMapper);

                XFormatter = p => new DateTime((long)p).ToString("d");
                OnPropertyChanged(nameof(SeriesCollection));
                OnPropertyChanged(nameof(TotalPriceForPeriod));
            });
        }
    }
}
