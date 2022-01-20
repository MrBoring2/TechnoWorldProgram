
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Programm.Services;
using TechnoWorld_Programm.ViewModels.Windows;
using TechnoWorld_Programm.Views.Windows;

namespace TechnoWorld_Programm
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
            WindowNavigation.Instance.RegisterWindow<ElectronicDetailWindowVM, ElectronicsDetailWindow>();
        }
    }
}
