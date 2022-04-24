using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;

namespace TechnoWorld_WarehouseAccounting.Services
{
    public class PageController
    {
        private readonly Dictionary<Type, Type> viewModelsToPagesMapping = new Dictionary<Type, Type>();
        private readonly Dictionary<BasePageVM, Page> createdPages = new Dictionary<BasePageVM, Page>();

        public IEnumerable<BasePageVM> Pages => createdPages.Keys;

        public void RegisterPageType<VM, P>()
            where P : Page
            where VM : BasePageVM
        {
            var vmType = typeof(VM);
            viewModelsToPagesMapping[vmType] = typeof(P);           
        }

        public void CreatePage(Type vmType)
        {
            var vm = (BasePageVM)Activator.CreateInstance(vmType);
            var page = CreatePageInstanceWithVM(vm);
            createdPages[vm] = page;
        }

        public void UnregisterPageType<VM>()
        {
            var vmType = typeof(VM);
            if (!viewModelsToPagesMapping.ContainsKey(vmType))
                throw new InvalidOperationException(
                    $"Тип {vmType.FullName} не зарегистрирован");
            viewModelsToPagesMapping.Remove(vmType);
        }

        public Page CreatePageInstanceWithVM(BasePageVM vm)
        {
            if (vm == null)
                throw new ArgumentNullException("ViewModel is null");
            Type pageType = null;

            var vmType = vm.GetType();
            while (vmType != null && !viewModelsToPagesMapping.TryGetValue(vmType, out pageType))
                vmType = vmType.BaseType;

            if (pageType == null)
                throw new ArgumentException(
                    $"Нет зарегистрированого окна с таким ViewModel: {vm.GetType().FullName}");

            var page = (Page)Activator.CreateInstance(pageType);
            page.DataContext = vm;
            return page;
        }
        public Page GetNewPage(BasePageVM vmPage)
        {
            //if (vm is null)
            //    throw new ArgumentNullException(nameof(vm));

            Page page;

            page = CreatePageInstanceWithVM(vmPage);
            return page;

        }
        public Page GetPage(Type vmPageType)
        {
            //if (vm is null)
            //    throw new ArgumentNullException(nameof(vm));

            Page page;
            if (createdPages.Keys.FirstOrDefault(p => p.GetType() == vmPageType) != null)
            {
                createdPages.TryGetValue(createdPages.Keys.FirstOrDefault(p => p.GetType() == vmPageType), out page);
                return page;
            }
            else
            {
                page = CreatePageInstanceWithVM((BasePageVM)Activator.CreateInstance(vmPageType));
                createdPages[(BasePageVM)Activator.CreateInstance(vmPageType)] = page;
                return page;
            }
        }

        public void HidePage(BasePageVM vm)
        {
            if (!createdPages.TryGetValue(vm, out var page))
                throw new InvalidOperationException(nameof(vm));

            createdPages.Remove(vm);
        }

        public Page GetFirstPage()
        {
            //if (createdPages.Count == 0)
            //    GetPage(viewModelsToPagesMapping.FirstOrDefault().Key.GetType());
            throw new NotImplementedException();
            //return createdPages.FirstOrDefault().Value;
        }

        public Page GetLastPage()
        {
            throw new NotImplementedException();
            //if (createdPages.Count == 0)
            //    GetPage(viewModelsToPagesMapping.LastOrDefault().Key.GetType());

            //return createdPages.LastOrDefault().Value;
        }
    }
}