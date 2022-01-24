﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;

using BNS_Programm.CustomElements;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using TechnoWorld_Terminal.ViewModels.Pages;
using TechnoWorld_Terminal.Services;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Views.Pages;
using System.Windows;

namespace TechnoWorld_Terminal.ViewModels.Windows
{
    public class MainAppWindowVM : WindowWithPagesVMBase
    {
        protected List<PageVMBase> _pageVMs;
        private ItemMenu selectedMenuItem;
        public MainAppWindowVM()
        {
            ClientService.Instance.RestClient = new RestClient(ApiService.apiUrl);
            RegisterPages();
            InitializeMenu();

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
                SwitchPage(SelectedMenuItem.TargetPageVM);
                OnPropertyChanged();
            }
        }


        public ObservableCollection<ItemMenu> MenuItems { get; set; }
        public ObservableCollection<ItemMenu> OptionalMenuItems { get; set; }
        private void RegisterPages()
        {
            RegisterPageWithVM<ElectronicsListPageVM, ElectronicsListPage>();
            RegisterPageWithVM<CartPageVM, CartPage>();
            RegisterPageWithVM<CategoriesPageVM, CategoriesPage>();
        }
        private void InitializeMenu()
        {
            MenuItems = new ObservableCollection<ItemMenu>
            {
                new ItemMenu("Список товаров", PackIconKind.Shop, GetPageInstance(typeof(CategoriesPageVM))), //),
                new ItemMenu("Корзина", PackIconKind.Cart, GetPageInstance(typeof(CartPageVM))),
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

           // MessageBox.Show(CurrentPage.GetType().ToString());
        }


        private void Exit()
        {
            WindowNavigation.Instance.CloseWindow(this);
        }

    }
}
