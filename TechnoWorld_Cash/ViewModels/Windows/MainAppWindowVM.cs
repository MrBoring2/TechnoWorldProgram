using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Cash.Services;
using TechnoWorld_Cash.ViewModels.Pages;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_Cash.ViewModels.Windows
{
    public class MainAppWindowVM : BaseWindowVM
    {
        public MainAppWindowVM()
        {
            Initialize();
        }
        public RelayCommand WindowLoadedCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        private void Initialize()
        {
            WindowLoadedCommand = new RelayCommand(WindowLoaded);
            ExitCommand = new RelayCommand(Exit);
        }

        private void WindowLoaded(object obj)
        {
            PageNavigation.Instance.RegisterPages();
            PageNavigation.Navigate(typeof(CashPageVM));
        }

        private void Exit(object obj)
        {
            ApiService.Instance.ShutDownService();
            WindowNavigation.Instance.OpenAndHideWindow(this, new LoginWindowViewModel());
        }
    }
}
