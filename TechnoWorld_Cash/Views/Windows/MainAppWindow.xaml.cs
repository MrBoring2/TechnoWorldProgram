using MaterialDesignExtensions.Controls;
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
using System.Windows.Shapes;
using TechnoWorld_Cash.Services;

namespace TechnoWorld_Cash.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : MaterialWindow
    {
        public MainAppWindow()
        {
            InitializeComponent();
            PageNavigation.Service = MainFrame.NavigationService;
            WindowState = WindowState.Maximized;
        }
    }
}
