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
using TechnoWorld_WarehouseAccounting.Services;

namespace TechnoWorld_WarehouseAccounting.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : MaterialWindow
    {
        public MainAppWindow()
        {
            InitializeComponent();
            PageNavigation.Service = MainFraim.NavigationService;
            this.WindowState = WindowState.Maximized;
        }

        private void menu_HamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            //if (!menu.IsPaneOpen)
            //{
            //    System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect();
            
            //    MainFraim.Effect = objBlur;
            //    //MainFraim.Opacity = 0.5;
            //}
            //else
            //{
            //    //MainFraim.Opacity = 1;
            //    MainFraim.Effect = null;
            //}
        }
    }
}
