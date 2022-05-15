using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!Directory.Exists("Приходные накладные"))
            {
                Directory.CreateDirectory("Приходные накладные");
            }
            if (!Directory.Exists("Инвентаризация"))
            {
                Directory.CreateDirectory("Инвентаризация");
            }

            RegisterWindows();
            var loginWindow = new LoginWindowVM();
            WindowNavigation.Instance.OpenWindow(loginWindow);
            ApiService.Instance.GetHubConnection.Closed += HubConnection_Closed;
        }
        private Task HubConnection_Closed(Exception arg)
        {
            return App.Current.Dispatcher.InvokeAsync(() =>
            {
                if (arg != null)
                {
                    MaterialNotification.Show("Критическая ошибка", "Потеряно соединение с сервером!", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    WindowNavigation.Instance.OpenWindow(new LoginWindowVM());
                    PageNavigation.Instance.ClearCreatedPages();
                    WindowNavigation.Instance.CloseWindows();
                }

            }).Task;
        }

        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<LoginWindowVM, LoginWindow>();
            WindowNavigation.Instance.RegisterWindow<MainAppWindowVM, MainAppWindow>();
            WindowNavigation.Instance.RegisterWindow<ProductWindowVM, ProductWindow>();
            WindowNavigation.Instance.RegisterWindow<DeliveryWindowVM, DeliveryWindow>();
            WindowNavigation.Instance.RegisterWindow<ProductListWindowVM, ProductsListWindow>();
            WindowNavigation.Instance.RegisterWindow<DestributionOrderWindowVM, DestributionOrderWindow>();
            WindowNavigation.Instance.RegisterWindow<EmployeeWindowVM, EmployeeWindow>();
        }
    }
}
