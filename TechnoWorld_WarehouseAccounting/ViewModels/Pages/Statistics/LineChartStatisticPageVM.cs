using LiveCharts;
using LiveCharts.Configurations;
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
    public class LineChartStatisticPageVM : BasePageVM
    {
        private SeriesCollection seriesCollection;
        private ObservableCollection<string> labels;
        private Func<double, string> yFormatter;
        private DateTime _startDate;
        private DateTime _endDate;
        public LineChartStatisticPageVM(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
            CreateChart();
        }
        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Labels { get => labels; set { labels = value; OnPropertyChanged(); } }
        public Func<double, string> YFormatter { get => yFormatter; set { yFormatter = value; OnPropertyChanged(); } }
        public async void CreateChart()
        {
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

            if (responseOrders.StatusCode == System.Net.HttpStatusCode.OK && responseTypes.StatusCode == System.Net.HttpStatusCode.OK &&
                responseCategories.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseOrders.Content);
                var types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(responseTypes.Content);
                var categories = JsonConvert.DeserializeObject<List<OrderElectronic>>(responseCategories.Content);



                foreach (var type in types)
                {
                   
                    ChartValues<TypeSales> values = new ChartValues<TypeSales>();
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
                        values.Add(new TypeSales(type, count, sales));
                    }
                    SeriesCollection.Add(new LineSeries
                    {
                        Title = type.Name,
                        Values = values , DataLabels = true                                   
                    });
                }
            }

           
           
            for (DateTime date = _startDate; date <= _endDate; date = date.AddDays(1))
            {
                Labels.Add(date.ToShortDateString());
            }

            var typesMapper = Mappers.Xy<TypeSales>()
                .X((value, index) => index)
                .Y(value => value.Count);

            Charting.For<TypeSales>(typesMapper);

            OnPropertyChanged(nameof(SeriesCollection));
        }
    }
}
