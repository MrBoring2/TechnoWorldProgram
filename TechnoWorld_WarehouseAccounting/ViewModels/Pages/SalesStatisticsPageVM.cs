using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class SalesStatisticsPageVM : BasePageVM
    {
        private SeriesCollection seriesCollection;
        private ObservableCollection<string> labels;
        private Func<double, string> yFormatter;
        public SalesStatisticsPageVM()
        {
            CreateChartCommand = new RelayCommand(CreateChart);
        }
        public RelayCommand CreateChartCommand { get; set; }
        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Labels { get => labels; set { labels = value; OnPropertyChanged(); } }
        public Func<double, string> YFormatter { get => yFormatter; set { yFormatter = value; OnPropertyChanged(); } }

        private async void CreateChart(object obj)
        {
            var startDate = DateTime.Now.Date.AddMonths(-2);
            var endDate = DateTime.Now.Date.AddMonths(1);
            var responseElectronics = await ApiService.Instance.GetRequestWithParameter("api/Orders/ForStatistics", "chartParams", new
            {
                electronicsTypeId = 0,
                categoryId = 1,
                startDate = startDate,
                endDate = endDate
            });

            var responseTypes = await ApiService.Instance.GetRequest("api/ElectrnicsTypes");
            var responseCategories = await ApiService.Instance.GetRequest("api/Categories");

            if (responseElectronics.StatusCode == System.Net.HttpStatusCode.OK && responseTypes.StatusCode == System.Net.HttpStatusCode.OK &&
                responseCategories.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var electroncis = JsonConvert.DeserializeObject<List<OrderElectronic>>(responseElectronics.Content);
                var types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(responseTypes.Content);
                var categories = JsonConvert.DeserializeObject<List<OrderElectronic>>(responseCategories.Content);

                for (DateTime d = startDate; startDate < endDate; startDate.AddDays(1))
                {

                    foreach (var type in types)
                    {
                        ChartValues<double> values = new ChartValues<double>();
                        foreach (var electronics in electroncis.Where(p => p.Electronics.TypeId == type.TypeId))
                        {
                            values.Add(electroncis.Count);
                        }
                        SeriesCollection.Add(new LineSeries
                        {
                            Title = type.Name,
                            Values = values
                        });
                    }
                    Labels.Add(d.ToShortDateString());
                }

                OnPropertyChanged(nameof(SeriesCollection));
            }
        }
    }
}
