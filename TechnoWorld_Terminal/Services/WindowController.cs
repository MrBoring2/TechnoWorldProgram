using MaterialDesignExtensions.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.ViewModels.Windows;
using TechnoWorld_Terminal.Views.Windows;

namespace TechnoWorld_Terminal.Services
{
    public class WindowController
    {
        private Dictionary<Type, Type> viewModelsToWindowsMapping = new Dictionary<Type, Type>();

        public void RegisterWindowType<VM, Win>()
            where Win : MaterialWindow
            where VM : WindowVMBase
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

        public MaterialWindow CreateWindowInstanceWithVM(WindowVMBase vm)
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


        Dictionary<WindowVMBase, MaterialWindow> openWindows = new Dictionary<WindowVMBase, MaterialWindow>();
        public void ShowPresentation(WindowVMBase windowViewModel)
        {
            if (windowViewModel == null)
                throw new ArgumentNullException("ViewModel is null");
            if (openWindows.ContainsKey(windowViewModel))
                throw new InvalidOperationException("Окно с тиким ViewModel уже отображено");
            var window = CreateWindowInstanceWithVM(windowViewModel);
            window.Show();
            openWindows[windowViewModel] = window;
        }

        public void HidePresentation(WindowVMBase windowViewModel)
        {
            MaterialWindow window;
            if (!openWindows.TryGetValue(windowViewModel, out window))
                throw new InvalidOperationException("Окно с тиким ViewModel не отображено");
            window.Close();
            openWindows.Remove(windowViewModel);
        }

        public void ShowModalPresentation(ModalWindowVMBase vm)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = CreateWindowInstanceWithVM(vm);
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ShowDialog();
            });
        }
    }
}
