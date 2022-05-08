using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TechnoWorld_Terminal.ViewModels.Pages;
using WPF_Helpers.Abstractions;

namespace TechnoWorld_Terminal.ViewModels
{
    public class ItemMenu
    {
        public ItemMenu(string title, PackIconKind icon, BasePageVM targetPageVM)
        {
            Title = title;
            Icon = icon;
            TargetPageVM = targetPageVM;
        }

        public string Title { get; set; }
        public PackIconKind Icon { get; set; }
        public BasePageVM TargetPageVM { get; set; }
    }
}
