using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TechnoWorld_API.Models;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;
using TechnoWorld_WarehouseAccounting.Views.Pages;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_Helpers.Services;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class ProductListWindowVM : BaseModalWindowVM
    {
        public ProductListWindowVM()
        {
            var productManagementPageVM = new ProductManagementPageVM(true);
            productManagementPageVM.onElectronicSelected += ProductManagementPageVM_onElectronicSelected;
            Content = PageNavigation.GetNewPage(productManagementPageVM);

        }

        private void ProductManagementPageVM_onElectronicSelected(object sender, EventArgs e)
        {
            DialogResult = true;
        }

        public Page Content { get; set; }
        public Electronic SelectedElectronic => (Content.DataContext as ProductManagementPageVM).SelectedEntity;
    }
}
