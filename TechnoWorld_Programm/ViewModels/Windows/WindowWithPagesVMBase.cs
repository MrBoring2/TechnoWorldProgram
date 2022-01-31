using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Services;
using TechnoWorld_Terminal.ViewModels.Pages;

namespace TechnoWorld_Terminal.ViewModels.Windows
{
    public class WindowWithPagesVMBase : WindowVMBase
    {
        private readonly PageController pageController;
        private Page currentPage;

        public WindowWithPagesVMBase()
        {
            pageController = new PageController();

            ChangePageCommand = new RelayCommand(SwitchPage);
            ClosePageCommand = new RelayCommand(ClosePage);
        }

        public IEnumerable<PageVMBase> Pages => pageController.Pages;

        public Page CurrentPage
        {
            get => currentPage;
            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand ChangePageCommand { get; }
        public RelayCommand ClosePageCommand { get; }

        protected void RegisterPageWithVM<VM, Pag>()
            where VM : PageVMBase
            where Pag : Page
        {
            pageController.RegisterPageType<VM, Pag>();

            if(CurrentPage is null)
            {
                SwitchPage((PageVMBase)pageController.GetFirstPage().DataContext);
            }
        }

        protected PageVMBase GetPageInstance(Type vmType)
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

        protected void SwitchPage(object vm)
        {
            CurrentPage = pageController.GetPage(vm as PageVMBase);
        }

        private void ClosePage(object param)
        {
            PageVMBase vmForClose = (PageVMBase)CurrentPage.DataContext;
            CurrentPage = pageController.GetLastPage();
            pageController.HidePage(vmForClose);
        }
    }
}
