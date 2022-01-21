
using MaterialDesignExtensions.Controls;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using TechnoWorld_Programm.POCO_Models;

namespace TechnoWorld_Terminal.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ElectronicsDetailWindow.xaml
    /// </summary>
    public partial class ElectronicsDetailWindow : MaterialWindow
    {
     

        public ElectronicsDetailWindow(Electronic electronic)
        {
            InitializeComponent();
        }


        

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //this.DialogResult = false;
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            //if (ClientService.Instance.Cart.FirstOrDefault(p => p.Electronic.Equals(CurrentElectronic)) == null)
            //{
            //    ClientService.Instance.Cart.Add(new Models.CartItem(CurrentElectronic));
            //    this.DialogResult = true;
            //}
            //else
            //{
            //    var notoficationManager = new NotificationManager();
            //    notoficationManager.Show(new NotificationContent
            //    {
            //        Title = "Внимание",
            //        Message = $"Товар {CurrentElectronic.Model} уже находится в корзине!",
            //        Type = NotificationType.Warning
            //    }, areaName: "ElectronicNotificationArea");
            //}
        }
    }
}
