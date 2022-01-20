
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TechnoWorld_Programm.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для CartPage.xaml
    /// </summary>
    public partial class CartPage : Page
    {
        

        //MainAppWindow context
        public CartPage()
        {
            InitializeComponent();
          
           // Pickup.IsChecked = true;
     //        _context = context;
       //     DataContext = this;
        }

 

       

      

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_context.SwitchPage(_context.MenuItems.FirstOrDefault(p => p.Page.GetType() == typeof(ElectronicsListPage)).Page);
        }

        private void Pickup_Checked(object sender, RoutedEventArgs e)
        {
            //DeliveryPrice = 0;
            //DeliveryAddressVisibility = Visibility.Hidden;
            //UpdatePayment();
        }

        private void Delivery_Checked(object sender, RoutedEventArgs e)
        {
            //DeliveryPrice = 400;
            //DeliveryAddressVisibility = Visibility.Visible;
            //UpdatePayment();
        }
    }
}
