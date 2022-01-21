using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnoWorld_Terminal.ViewModels.Pages;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechnoWorld_Terminal.Views.Windows;

namespace TechnoWorld_Terminal.Services
{
    public class PageController
    {
        private readonly Dictionary<Type, Type> viewModelsToPagesMapping = new Dictionary<Type, Type>();
        private readonly Dictionary<PageVMBase, Page> createdPages = new Dictionary<PageVMBase, Page>();

        public IEnumerable<PageVMBase> Pages => createdPages.Keys;

        public void RegisterPageType<VM, P>()
            where P : Page
            where VM : PageVMBase
        {
            var vmType = typeof(VM);
            viewModelsToPagesMapping[vmType] = typeof(P);

            var vm = (PageVMBase)Activator.CreateInstance(vmType);
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

        public Page CreatePageInstanceWithVM(PageVMBase vm)
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
        public Page GetPage(PageVMBase vm)
        {
            if (vm is null)
                throw new ArgumentNullException(nameof(vm));

            if (createdPages.TryGetValue(vm, out var page))
                return page;

            page = CreatePageInstanceWithVM(vm);
            createdPages[vm] = page;
            return page;
        }

        public void HidePage(PageVMBase vm)
        {
            if (!createdPages.TryGetValue(vm, out var page))
                throw new InvalidOperationException(nameof(vm));

            createdPages.Remove(vm);
        }

        public Page GetFirstPage()
        {
            if (createdPages.Count == 0)
                GetPage((PageVMBase)Activator.CreateInstance(viewModelsToPagesMapping.FirstOrDefault().Key));

            return createdPages.FirstOrDefault().Value;
        }

        public Page GetLastPage()
        {
            if (createdPages.Count == 0)
                GetPage((PageVMBase)Activator.CreateInstance(viewModelsToPagesMapping.LastOrDefault().Key));

            return createdPages.LastOrDefault().Value;
        }
    }
}
