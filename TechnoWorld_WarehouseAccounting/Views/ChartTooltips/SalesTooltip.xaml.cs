using LiveCharts;
using LiveCharts.Definitions.Charts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using TechnoWorld_WarehouseAccounting.Models.ForStatistics;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics;

namespace TechnoWorld_WarehouseAccounting.Views.ChartTooltips
{
    /// <summary>
    /// Логика взаимодействия для TypesTooltip.xaml
    /// </summary>
    public partial class SalesTooltip : UserControl, IChartTooltip
    {

        private TooltipData _data;
        public SalesTooltip()
        {
            InitializeComponent();
            DataContext = this;
        }
        public TooltipData Data
        {

            get
            {


                return _data;
            }
            set
            {
                foreach (var point in value.Points.ToList())
                {
                    var dataContext = ((value.SenderSeries.Parent as Canvas).Parent as CartesianChart).DataContext as LineChartStatisticPageVM;
                    var px = (long)point.ChartPoint.X;

                    var d = dataContext.lastXValue;
                    if ((long)point.ChartPoint.X != dataContext.lastXValue)
                    {
                        value.Points.Remove(point);
                    }
                }

                _data = value;
                OnPropertyChanged("Data");
                OnPropertyChanged("Title");

            }
        }
        public string Title => Data == null || Data.Points.Count < 0 ? "" : new DateTime((long)Data.Points.FirstOrDefault().ChartPoint.X).ToShortDateString();
        public TooltipSelectionMode? SelectionMode { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
