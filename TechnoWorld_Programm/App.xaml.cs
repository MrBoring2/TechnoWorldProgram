
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
        }

        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<MainAppWindowVM, MainAppWindow>();
        }
    }
}
