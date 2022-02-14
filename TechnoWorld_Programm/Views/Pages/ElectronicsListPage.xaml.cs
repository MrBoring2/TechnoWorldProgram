

using TechnoWorld_Terminal.ViewModels;

using Notification.Wpf;
using Notification.Wpf.Classes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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
using TechnoWorld_Terminal.Views.Windows;

namespace TechnoWorld_Terminal.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ElectronicsListWindow.xaml
    /// </summary>
    public partial class ElectronicsListPage : Page
    {


        public ElectronicsListPage()
        {
            InitializeComponent();
           

            //DataContext = this;


        }





        private void PagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          //  RefrashPaginator();
          //  RefreshElectronics();
        }

        private void ToLast_Click(object sender, RoutedEventArgs e)
        {
         //   SelectedPageNumber = PagesNumbers.Count;
         //   RefrashPaginator();
        }

        private void ToFirst_Click(object sender, RoutedEventArgs e)
        {
          //  SelectedPageNumber = 1;
        //    RefrashPaginator();
        }

        private void isDesceningCheck_Changed(object sender, RoutedEventArgs e)
        {
          //  var sort = SortParameters.FirstOrDefault(p => p.Title.Equals(SelectedSort.Title));
          //  RefreshElectronics();
        }


        private void TypeSelect_Changed(object sender, RoutedEventArgs e)
        {
         //   RefreshElectronics();
        //    RefreshPages();
        }

        private void ManufacturerSelect_Changed(object sender, RoutedEventArgs e)
        {
          //  RefreshElectronics();
           // RefreshPages();
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
         //   var electorincDetailWindow = new ElectronicsDetailWindow(SelectedElectronic);
            //if (electorincDetailWindow.ShowDialog() == true)
            //{
            //    var notificationManager = new NotificationManager();
            //    notificationManager.Show(new NotificationContent
            //    {
            //        Title = "Оповещение",
            //     //   Message = $"Товар {SelectedElectronic.Model} добавлен в корзину!",
            //        Type = NotificationType.Information
            //    }, areaName: "MainNotificationArea");
            //}
        }

        private void Max_MouseUp(object sender, MouseButtonEventArgs e)
        {

         //   RefreshElectronics();
         //   RefreshPages();
        }

        private void Min_MouseDown(object sender, MouseButtonEventArgs e)
        {

        //    RefreshElectronics();
          //  RefreshPages();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //RefreshElectronics();
            //RefreshPages();
        }

    }
}
