using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using TechnoWorld_Terminal.ViewModels.Pages;
using TechnoWorld_Terminal.Views.Pages;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_Terminal.Services
{
    public class PageNavigation
    {
        private static readonly PageController pageController = new PageController();
        private static volatile PageNavigation instance;
        private static object syncRoot = new object();
        private NavigationService _navService;

        private PageNavigation() { RegisterPages(); }
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

        public void ClearCreatedPages()
        {
            pageController.ClearPages();
        }
        protected void RegisterPageWithVM<VM, Pag>()
            where VM : BasePageVM
            where Pag : Page
        {
            pageController.RegisterPageType<VM, Pag>();
        }
        public static bool IsPageExist(Type vmType)
        {
            if (vmType != null)
            {
                return pageController.Pages.FirstOrDefault(p => p.GetType().Equals(vmType)) != null;
            }
            else
            {
                return false;
            }
        }
        private void _navService_Navigated(object sender, NavigationEventArgs e)
        {
        }
        public static Page GetPage(Type pageVMType)
        {
            return pageController.GetPage(pageVMType);
        }
        public static Page GetNewPage(BasePageVM pageVM)
        {
            return pageController.GetNewPage(pageVM);
        }
        public static void HidePage(Type vmPageType)
        {
            pageController.HidePage(vmPageType);
        }
        public static void Navigate(Type pageVMtype)
        {
            if (Instance._navService != null && pageVMtype != null)
            {
                Page page = pageController.GetPage(pageVMtype);

                Instance._navService.Navigate(page);
            }
        }
        public static void Navigate(Type pageVMtype, params object[] args)
        {
            if (Instance._navService != null && pageVMtype != null)
            {
                Page page = pageController.GetPage(pageVMtype, args);

                Instance._navService.Navigate(page);
            }
        }
        public static void NavigateToNewPage(BasePageVM pageVM)
        {
            if (Instance._navService != null && pageVM != null)
            {
                Page page = pageController.GetNewPage(pageVM);

                Instance._navService.Navigate(page);
            }
        }

        private void RegisterPages()
        {
            RegisterPageWithVM<ElectronicsListPageVM, ElectronicsListPage>();
            RegisterPageWithVM<CartPageVM, CartPage>();
            RegisterPageWithVM<CategoriesPageVM, CategoriesPage>();
            RegisterPageWithVM<ElectronicsDetailPageVM, ElectronicsDetailPage>();
        }
    }
}
