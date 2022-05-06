
using MaterialDesignExtensions.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.ViewModels.Windows;

namespace TechnoWorld_Terminal.Services
{
    internal class WindowNavigation
    {
        private static WindowNavigation insance;
        private WindowController displayRootRegistry = new WindowController();
        private static object syncRoot = new Object();

        public static WindowNavigation Instance
        {
            get
            {
                if (insance == null)
                {
                    lock (syncRoot)
                    {
                        if (insance is null)
                            insance = new WindowNavigation();
                    }
                }
                return insance;
            }
        }

        public void RegisterWindow<VM, Win>()
            where VM : BaseWindowVM
            where Win : MaterialWindow
        {
            displayRootRegistry.RegisterWindowType<VM, Win>();
        }

        public void OpenWindow(BaseWindowVM newWindowVM)
        {

            if (newWindowVM != null)
            {
                displayRootRegistry.ShowPresentation(newWindowVM);
            }
        }

        public void OpenAndHideWindow(BaseWindowVM currentWindowVM, BaseWindowVM newWindowVM)
        {
            if (currentWindowVM != null && newWindowVM != null)
            {
                displayRootRegistry.ShowPresentation(newWindowVM);
                displayRootRegistry.HidePresentation(currentWindowVM);
            }
        }
        public void CloseWindow(BaseWindowVM currentWindowVM)
        {
            if (currentWindowVM != null)
            {
                displayRootRegistry.HidePresentation(currentWindowVM);
            }
        }
        public void OpenModalWindow(ModalWindowVMBase windowVM)
        {
            if (windowVM != null)
            {
                displayRootRegistry.ShowModalPresentation(windowVM);
            }
        }
        public void CloseWindows()
        {
            displayRootRegistry.CloseAllWindow();
        }
    }
}
