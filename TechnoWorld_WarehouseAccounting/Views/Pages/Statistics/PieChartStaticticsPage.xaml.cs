using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TechnoWorld_WarehouseAccounting.Views.Pages.Statistics
{
    /// <summary>
    /// Логика взаимодействия для PieChartStaticticsPage.xaml
    /// </summary>
    public partial class PieChartStaticticsPage : Page
    {
        public PieChartStaticticsPage()
        {
            InitializeComponent();
        }

        private void lineChart_DataClick(object sender, LiveCharts.ChartPoint chartPoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartPoint.SeriesView;
            selectedSeries.PushOut = 8;

        }
    }
}
