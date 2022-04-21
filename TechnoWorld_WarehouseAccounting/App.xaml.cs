﻿using System;
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
            var mainAppWindowVM = new MainAppWindowVM();
            WindowNavigation.Instance.OpenWindow(mainAppWindowVM);
        }

        private void RegisterWindows()
        {
            WindowNavigation.Instance.RegisterWindow<MainAppWindowVM, MainAppWindow>();
        }
    }
}