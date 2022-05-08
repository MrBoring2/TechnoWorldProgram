
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.Services;
using TechnoWorld_Terminal.ViewModels.Pages;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechnoWorld_Terminal.Views.Windows;
using WPF_VM_Abstractions;

namespace TechnoWorld_Terminal
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            RegisterWindows();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindowVM = new MainAppWindowVM();
            WindowNavigation.Instance.OpenWindow(mainWindowVM);
            ApiService.Instance.GetHubConnection.Closed += HubConnection_Closed;
        }
        private Task HubConnection_Closed(Exception arg)
        {
            return Current.Dispatcher.InvokeAsync(() =>
            {
                if (arg != null)
                {
                    //CustomMessageBox.Show("Потеряно соединение с сервером!", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);                  
                    PageNavigation.Instance.ClearCreatedPages();
                    ShutdownMode = ShutdownMode.OnExplicitShutdown;
                    WindowNavigation.Instance.CloseWindows();
                    WindowNavigation.Instance.OpenWindow(new MainAppWindowVM());
                    ShutdownMode = ShutdownMode.OnLastWindowClose;
                }

            }).Task;
        }


        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<MainAppWindowVM, MainAppWindow>();
        }
    }
}
