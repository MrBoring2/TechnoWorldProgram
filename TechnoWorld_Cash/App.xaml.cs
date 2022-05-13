using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Cash.ViewModels.Windows;
using TechnoWorld_Cash.Views.Windows;
using TechnoWorld_Cash.Services;
using WPF_VM_Abstractions;
using System.IO;
using MaterialNotificationLibrary;
using MaterialNotificationLibrary.Enums;

namespace TechnoWorld_Cash
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (!Directory.Exists("Чеки"))
            {
                Directory.CreateDirectory("Чеки");
            }
            RegisterWindows();
            var loginWindowVM = new LoginWindowViewModel();
            WindowNavigation.Instance.OpenWindow(loginWindowVM);
            ApiService.Instance.GetHubConnection.Closed += HubConnection_Closed;
        }
        private Task HubConnection_Closed(Exception arg)
        {
            return Current.Dispatcher.InvokeAsync(() =>
            {
                if (arg != null)
                {
                    MaterialNotification.Show("Критическая ошибка", "Потеряно соединение с сервером!", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    WindowNavigation.Instance.OpenWindow(new LoginWindowViewModel());
                    PageNavigation.Instance.ClearCreatedPages();
                    WindowNavigation.Instance.CloseWindows();
                }

            }).Task;
        }
        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<LoginWindowViewModel, LoginWindow>();
            WindowNavigation.Instance.RegisterWindow<PaymentWindowViewModel, PaymentWindow>();
            WindowNavigation.Instance.RegisterWindow<MainAppWindowVM, MainAppWindow>();
        }
    }
}
