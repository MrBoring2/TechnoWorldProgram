using LiveCharts;
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

namespace TechnoWorld_WarehouseAccounting.Views.ChartTooltips
{
    /// <summary>
    /// Логика взаимодействия для TypesTooltip.xaml
    /// </summary>
    public partial class TypesTooltip : UserControl, IChartTooltip
    {
        private TooltipData _data;
        public TypesTooltip()
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

                _data = value;
                OnPropertyChanged("Data");

            }
        }
        public TooltipSelectionMode? SelectionMode { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
