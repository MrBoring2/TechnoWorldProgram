using MaterialDesignExtensions.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechnoWorld_Terminal.Views.Windows;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_Terminal.Services
{
    public class WindowController
    {
        private Dictionary<Type, Type> viewModelsToWindowsMapping = new Dictionary<Type, Type>();

        public void RegisterWindowType<VM, Win>()
            where Win : MaterialWindow
            where VM : BaseWindowVM
        {
            var vmType = typeof(VM);
            viewModelsToWindowsMapping[vmType] = typeof(Win);
        }

        public void UnregisterWindowType<VM>()
        {
            var vmType = typeof(VM);
            if (!viewModelsToWindowsMapping.ContainsKey(vmType))
                throw new InvalidOperationException(
                    $"Тип {vmType.FullName} не зарегистрирован");
            viewModelsToWindowsMapping.Remove(vmType);
        }

        public MaterialWindow CreateWindowInstanceWithVM(BaseWindowVM vm)
        {
            if (vm == null)
                throw new ArgumentNullException("ViewModel is null");
            Type windowType = null;

            var vmType = vm.GetType();
            while (vmType != null && !viewModelsToWindowsMapping.TryGetValue(vmType, out windowType))
                vmType = vmType.BaseType;

            if (windowType == null)
                throw new ArgumentException(
                    $"Нет зарегистрированого окна с таким ViewModel: {vm.GetType().FullName}");

            var window = (MaterialWindow)Activator.CreateInstance(windowType);
            window.DataContext = vm;
            return window;
        }


        Dictionary<BaseWindowVM, MaterialWindow> openWindows = new Dictionary<BaseWindowVM, MaterialWindow>();
        public void ShowPresentation(BaseWindowVM windowViewModel)
        {
            if (windowViewModel == null)
                throw new ArgumentNullException("ViewModel is null");
            if (openWindows.ContainsKey(windowViewModel))
                throw new InvalidOperationException("Окно с тиким ViewModel уже отображено");
            var window = CreateWindowInstanceWithVM(windowViewModel);
            window.Show();
            openWindows[windowViewModel] = window;
        }

        public void HidePresentation(BaseWindowVM windowViewModel)
        {
            MaterialWindow window;
            if (!openWindows.TryGetValue(windowViewModel, out window))
                throw new InvalidOperationException("Окно с тиким ViewModel не отображено");
            window.Close();
            openWindows.Remove(windowViewModel);
        }
        public void CloseAllWindow()
        {
            foreach (var item in openWindows)
            {
                //if (item.Key.GetType() != typeof(LoginWindowVM))
                //{
                item.Value.Close();
                //}
            }
        }
        public void ShowModalPresentation(BaseModalWindowVM vm)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                var window = CreateWindowInstanceWithVM(vm);
                openWindows[vm] = window;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ShowDialog();
                openWindows.Remove(vm);
            });
        }
    }
}
