using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;

namespace TechnoWorld_WarehouseAccounting.Models
{
    public class MenuItem
    {
        public MenuItem(string title, PackIconKind icon, Type pageDestination = null)
        {
            Title = title;
            Icon = icon;
            PageDestination = pageDestination;
        }

        public string Title { get; set; }
        public PackIconKind Icon { get; set; }
        public Type PageDestination { get; set; }
    }
}
