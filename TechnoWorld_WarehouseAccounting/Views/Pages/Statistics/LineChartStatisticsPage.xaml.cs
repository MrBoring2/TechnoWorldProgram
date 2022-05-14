using LiveCharts;
using LiveCharts.Helpers;
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
using TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics;

namespace TechnoWorld_WarehouseAccounting.Views.Pages.Statistics
{
    /// <summary>
    /// Логика взаимодействия для LineChartStatisticsPage.xaml
    /// </summary>
    public partial class LineChartStatisticsPage : Page
    {
        public LineChartStatisticsPage()
        {
            InitializeComponent();
        }

        private void lineChart_MouseMove(object sender, MouseEventArgs e)
        {
            var vm = (LineChartStatisticPageVM)DataContext;
            var chart = (LiveCharts.Wpf.CartesianChart)sender;

            //lets get where the mouse is at our chart
            var mouseCoordinate = e.GetPosition(chart);
            var p = chart.ConvertToChartValues(mouseCoordinate);
            if (chart.Series.Count <= 0)
            {
                return;
            }
            var series = chart.Series?[0];
            var closetsPoint = series.ClosestPointTo(p.X, AxisOrientation.X);


            (DataContext as LineChartStatisticPageVM).lastXValue = (long)closetsPoint.X;
        }
    }
}
