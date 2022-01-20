using TechnoWorld_Programm.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BNS_Programm.ViewModels;
using RestSharp;
using TechnoWorld_Programm.Services;
using BNS_Programm.CustomElements;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using TechnoWorld_Programm.ViewModels.Pages;
using BNS_Programm.Common;
using TechnoWorld_Programm.Views.Pages;

namespace TechnoWorld_Programm.ViewModels.Windows
{
    public class MainAppWindowVM : WindowVMBase
    {
        protected List<PageVMBase> _pageVMs;
        private ItemMenu selectedMenuItem;
        protected PageVMBase _currentPageVM;
        protected PageRegistrationService PagesRegistrator { get; set; }
        public MainAppWindowVM()
        {
           
            //var menuRegister = new List<SubItem>();
            //menuRegister.Add(new SubItem("Employee"));
            PagesRegistrator = new PageRegistrationService();
            ClientService.Instance.RestClient = new RestClient(ApiService.apiUrl);
            //RegisterPages();
            InitializeMenu();
            ChangePageCommand = new RelayCommand(ChangePage);
        }
        public PageVMBase CurrentPageVM
        {
            get => _currentPageVM;
            set
            {
                if (_currentPageVM != value)
                {
                    _currentPageVM = value;
                    OnPropertyChanged("CurrentPageVM");
                }
            }
        }
        public List<PageVMBase> PageVMs
        {
            get
            {
                if (_pageVMs == null)
                    _pageVMs = new List<PageVMBase>();
                return _pageVMs;
            }
        }
        public ItemMenu SelectedMenuItem
        {
            get
            {
                return selectedMenuItem;
            }
            set
            {
                selectedMenuItem = value;

                if (SelectedMenuItem.Title.Equals(OptionalMenuItems.FirstOrDefault().Title))
                {
                    Exit();
                }
                ChangePage(null);
                OnPropertyChanged();
            }
        }


        public ObservableCollection<ItemMenu> MenuItems { get; set; }
        public ObservableCollection<ItemMenu> OptionalMenuItems { get; set; }
        public RelayCommand ChangePageCommand { get; set; }
        private void InitializeMenu()
        {
            MenuItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu("Список товаров", PackIconKind.Shop, new ElectronicsListPageVM()),
                new ItemMenu("Корзина", PackIconKind.Cart, new CartPageVM()),
                new ItemMenu("Ваши заказы", PackIconKind.FileDocument, null),
                new ItemMenu("Гарантийное обслуживание", PackIconKind.AccountService, null)
            };
            OptionalMenuItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu("Выход", PackIconKind.ExitRun, null)
                {
                    Icon = PackIconKind.ExitToApp,
                    Title = "Выход"
                }
            };
            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        private void ChangePage(object obj)
        {
            if (SelectedMenuItem != null)
            {
                CurrentPageVM = SelectedMenuItem.TargetPageVM;
            }
            //if (!PageVMs.Contains(pageVM))
            //    PageVMs.Add(pageVM);

            //CurentPageVM = PageVMs
            //    .FirstOrDefault(vm => vm == pageVM);
        }
        //internal void SwitchPage(object sender)
        //{
        //    var screen = (Page)sender;

        //    if (screen != null)
        //    {
        //        //PageContainer.Navigate(sender);
        //    }
        //}
        private void Exit()
        {
            WindowNavigation.Instance.CloseWindow(this);
        }

    }
}
