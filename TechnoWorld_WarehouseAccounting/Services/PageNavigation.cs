﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;
using TechnoWorld_WarehouseAccounting.Views.Pages;

namespace TechnoWorld_WarehouseAccounting.Services
{
    public class PageNavigation
    {
        private static readonly PageController pageController = new PageController();
        private static volatile PageNavigation instance;
        private static object syncRoot = new Object();
        private NavigationService _navService;

        private PageNavigation() { }
        public static PageNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new PageNavigation();
                        }
                    }
                }
                return instance;
            }
        }
        public static NavigationService Service
        {
            get { return Instance._navService; }
            set
            {
                if (Instance._navService != null)
                {
                    Instance._navService.Navigated -= Instance._navService_Navigated;
                }

                Instance._navService = value;
                Instance._navService.Navigated += Instance._navService_Navigated;
            }
        }
        public void RegisterPages(int roleId)
        {
            switch (roleId)
            {
                case 1:
                    {

                    }break;
                case 2:
                    {

                    }break;
                case 3:
                    {
                        RegisterPageWithVM<ProductManagementPageVM, ProductManagementPage>();
                        CreatePage(typeof(ProductManagementPageVM));
                    }
                    break;
                case 4:
                    {
                        RegisterPageWithVM<ProductManagementPageVM, ProductManagementPage>();
                        CreatePage(typeof(ProductManagementPageVM));
                    }
                    break;
                default:
                    {

                    }break;
            }
            
            //RegisterPageWithVM<CartPageVM, CartPage>();
            //RegisterPageWithVM<CategoriesPageVM, CategoriesPage>();
            //RegisterPageWithVM<ElectronicsDetailPageVM, ElectronicsDetailPage>();
           
            //CreatePage(typeof(CartPageVM));
            //CreatePage(typeof(CategoriesPageVM));
        }
        protected void RegisterPageWithVM<VM, Pag>()
            where VM : BasePageVM
            where Pag : Page
        {
            pageController.RegisterPageType<VM, Pag>();

            //if (CurrentPage is null)
            //{
            //    //SwitchPage((PageVMBase)pageController.GetFirstPage().DataContext);
            //}
        }
        protected void CreatePage(Type vmType)
        {
            pageController.CreatePage(vmType);
        }
        protected BasePageVM GetPageInstance(Type vmType)
        {
            if (vmType != null)
            {
                return pageController.Pages.FirstOrDefault(p => p.GetType().Equals(vmType));
            }
            else
            {
                return pageController.Pages.FirstOrDefault();
            }
        }
        private void _navService_Navigated(object sender, NavigationEventArgs e)
        {
            //var page = e.Content as Page;

            //if (page == null)
            //{
            //    return;
            //}

            //page.DataContext = e.ExtraData;
        }
        public static Page GetPage(Type pageVMType)
        {
            return pageController.GetPage(pageVMType);
        }
        public static Page GetNewPage(BasePageVM pageVM)
        {
            return pageController.GetNewPage(pageVM);
        }
        public static void Navigate(Type pageVMtype)
        {
            if (Instance._navService != null && pageVMtype != null)
            {
                Page page = pageController.GetPage(pageVMtype);

                Instance._navService.Navigate(page);
            }
        }
        public static void Navigate(BasePageVM pageVM)
        {
            if (Instance._navService != null && pageVM != null)
            {
                Page page = pageController.GetNewPage(pageVM);

                Instance._navService.Navigate(page);
            }
        }
    }
}
