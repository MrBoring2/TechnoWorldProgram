using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnoWorld_Programm.ViewModels.Pages;
using TechnoWorld_Programm.ViewModels.Windows;
using TechnoWorld_Programm.Views.Windows;

namespace TechnoWorld_Programm.Services
{
    public class PageRegistrationService
    {
        private Dictionary<Type, Type> viewModelsToPagesMapping = new Dictionary<Type, Type>();

        public void RegisterPageType<VM, P>()
            where P : Page
            where VM : PageVMBase
        {
            var vmType = typeof(VM);
            viewModelsToPagesMapping[vmType] = typeof(P);
        }

        public void UnregisterPageType<VM>()
        {
            var vmType = typeof(VM);
            if (!viewModelsToPagesMapping.ContainsKey(vmType))
                throw new InvalidOperationException(
                    $"Тип {vmType.FullName} не зарегистрирован");
            viewModelsToPagesMapping.Remove(vmType);
        }

        public Page CreatePageInstanceWithVM(WindowVMBase vm)
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
    }
}
