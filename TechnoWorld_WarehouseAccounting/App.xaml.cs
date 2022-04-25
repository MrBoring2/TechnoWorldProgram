using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;

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

            RegisterWindows();
            var loginWindow = new LoginWindowVM();
            WindowNavigation.Instance.OpenWindow(loginWindow);
        }

        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<LoginWindowVM, LoginWindow>();
            WindowNavigation.Instance.RegisterWindow<MainAppWindowVM, MainAppWindow>();
            WindowNavigation.Instance.RegisterWindow<ProductWindowVM, ProductWindow>();
        }
    }
}
