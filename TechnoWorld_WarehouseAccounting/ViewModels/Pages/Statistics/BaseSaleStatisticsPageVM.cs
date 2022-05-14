using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Models.ForStatistics;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics
{
    public class BaseSaleStatisticsPageVM : BasePageVM
    {
        protected SeriesCollection seriesCollection;
        protected ObservableCollection<SaleModel> sales;
        protected ObservableCollection<string> labels;
        protected Func<double, string> yFormatter;
        protected Func<double, string> xFormatter;
        protected DateTime _startDate;
        protected DateTime _endDate;
        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Labels { get => labels; set { labels = value; OnPropertyChanged(); } }
        public ObservableCollection<SaleModel> Sales { get => sales; set { sales = value; OnPropertyChanged(); } }
        public Func<double, string> YFormatter { get => yFormatter; set { yFormatter = value; OnPropertyChanged(); } }
        public Func<double, string> XFormatter { get => yFormatter; set { yFormatter = value; OnPropertyChanged(); } }
        public long lastXValue { get; set; }
        public long From => _startDate.Ticks;
        public long To => _endDate.Ticks;
        public decimal TotalPriceForPeriod => Sales.Sum(p => p.TotalSales);
    }
}
