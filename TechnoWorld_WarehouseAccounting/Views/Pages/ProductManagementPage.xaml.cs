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
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;

namespace TechnoWorld_WarehouseAccounting.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductManagementPage.xaml
    /// </summary>
    public partial class ProductManagementPage : Page
    {
        public ProductManagementPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //PageNavigation.CreatePage(typeof(ProductManagementPageVM));
        }
    }
}
