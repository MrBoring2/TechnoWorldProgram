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

namespace TechnoWorld_Cash
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

            var loginWindowVM = new LoginWindowViewModel();
            WindowNavigation.Instance.OpenWindow(loginWindowVM);
        }

        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<LoginWindowViewModel, LoginWindow>();
            WindowNavigation.Instance.RegisterWindow<CashWindowViewModel, CashWindow>();
        }
    }
}
