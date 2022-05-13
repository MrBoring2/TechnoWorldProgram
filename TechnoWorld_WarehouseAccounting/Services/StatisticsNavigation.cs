using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages.Statistics;
using TechnoWorld_WarehouseAccounting.Views.Pages;
using TechnoWorld_WarehouseAccounting.Views.Pages.Statistics;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_WarehouseAccounting.Services
{
    public class StatisticsNavigation
    {
        private static readonly PageController pageController = new PageController();
        private static volatile StatisticsNavigation instance;
        private static object syncRoot = new Object();
        private NavigationService _navService;

        private StatisticsNavigation() { RegisterPages(); }
        public static StatisticsNavigation Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new StatisticsNavigation();
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
        public void RegisterPages()
        {
            RegisterPageWithVM<LineChartStatisticPageVM, LineChartStatisticsPage>();
            RegisterPageWithVM<PieChartStatisticsPageVM, PieChartStaticticsPage>();
            RegisterPageWithVM<ColumnChartStatisticsPageVM, ColumnChartStatisticsPage>();
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

            //if (CurrentPage is null)
            //{
            //    //SwitchPage((PageVMBase)pageController.GetFirstPage().DataContext);
            //}
        }
        public static void CreatePage(Type vmType)
        {
            if (!pageController.IsPageCreated(vmType))
            {
                pageController.CreatePage(vmType);
            }
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

