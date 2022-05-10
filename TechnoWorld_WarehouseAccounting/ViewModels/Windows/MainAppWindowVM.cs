using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Notification;
using TechnoWorld_Notification.Enums;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechnoWorld_WarehouseAccounting.ViewModels.Pages;
using TechnoWorld_WarehouseAccounting.ViewModels.Windows;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class MainAppWindowVM : BaseWindowVM
    {
        private MenuItem selectedMenuItem;
        private MenuItem selectedOptionsMenuItem;
        private string windowTitle;
        public MainAppWindowVM()
        {
            WindowLoadedCommand = new RelayCommand(WindowLoaded);
            LoadMenu();
        }

        public MenuItem SelectedMenuItem
        {
            get => selectedMenuItem;
            set
            {
                if (value != selectedMenuItem)
                {
                    selectedMenuItem = value;
                    OnPropertyChanged();
                    WindowTitle = $"ТЕХНО МИР | {selectedMenuItem.Title}";
                    if (selectedMenuItem != null)
                    {
                        PageNavigation.Navigate(selectedMenuItem.PageDestination);
                    }
                }
            }
        }

        public MenuItem SelectedOptionsMenuItem
        {
            get => selectedOptionsMenuItem;
            set
            {
                if (value != selectedOptionsMenuItem)
                {
                    selectedOptionsMenuItem = value;
                    OnPropertyChanged();
                    if (selectedOptionsMenuItem.Title == "Выход")
                    {
                        ClientService.Instance.Logout();
                        WindowNavigation.Instance.OpenAndHideWindow(this, new LoginWindowVM());
                    }
                }
            }
        }
        public string WindowTitle { get => windowTitle; set { windowTitle = value; OnPropertyChanged(); } }
        public RelayCommand WindowLoadedCommand { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public ObservableCollection<MenuItem> OptionalMenuItems { get; set; }
        public string CurrentUserFullName => $"Текущий пользователь: {ClientService.Instance.User.FullName}";
        public string CurrentUserPost => $"Должность: {ClientService.Instance.User.Post}";
        private void WindowLoaded(object obj)
        {
            PageNavigation.Instance.RegisterPages(ClientService.Instance.User.RoleId);
            SelectedMenuItem = MenuItems.FirstOrDefault(p => p.Title.Equals("Управление товарами"));
            MaterialNotification.Show("Оповещение", $"Добро пожаловать, {ClientService.Instance.User.FullName}.", MaterialNotificationButton.Ok, MaterialNotificationImage.Information);
        }
        private void LoadMenu()
        {
            MenuItems = new ObservableCollection<MenuItem>();
            switch (ClientService.Instance.User.RoleId)
            {
                case 2:
                    {
                        MenuItems.Add(new MenuItem("Инвентаризация", MaterialDesignThemes.Wpf.PackIconKind.ClipboardEditOutline));
                        MenuItems.Add(new MenuItem("Отчётная деятельность", MaterialDesignThemes.Wpf.PackIconKind.ChartBar));
                    }
                    break;
                case 3:
                    {
                        MenuItems.Add(new MenuItem("Управление товарами", MaterialDesignThemes.Wpf.PackIconKind.FormSelect, typeof(ProductManagementPageVM)));
                        MenuItems.Add(new MenuItem("Поставка товара", MaterialDesignThemes.Wpf.PackIconKind.BoxAdd, typeof(DeliveryManagementPageVM)));
                        MenuItems.Add(new MenuItem("Выдача товара", MaterialDesignThemes.Wpf.PackIconKind.Dolly, typeof(ProductDistributionPageVM)));
                    }
                    break;
                case 4:
                    {
                        MenuItems.Add(new MenuItem("Управление товарами", MaterialDesignThemes.Wpf.PackIconKind.FormSelect, typeof(ProductManagementPageVM)));
                        MenuItems.Add(new MenuItem("Управление сотрудниками", MaterialDesignThemes.Wpf.PackIconKind.Users));
                        MenuItems.Add(new MenuItem("Поставка товара", MaterialDesignThemes.Wpf.PackIconKind.BoxAdd, typeof(DeliveryManagementPageVM)));
                        MenuItems.Add(new MenuItem("Выдача товара", MaterialDesignThemes.Wpf.PackIconKind.Dolly, typeof(ProductDistributionPageVM)));
                        MenuItems.Add(new MenuItem("Инвентаризация", MaterialDesignThemes.Wpf.PackIconKind.ClipboardEditOutline));
                        MenuItems.Add(new MenuItem("Отчётная деятельность", MaterialDesignThemes.Wpf.PackIconKind.ChartBar));
                    }
                    break;
                default:
                    {
                        MaterialNotification.Show("Оповещение", $"Неккоректный пользователь.", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    }
                    break;
            }



            OptionalMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem("Выход", MaterialDesignThemes.Wpf.PackIconKind.ExitRun)
            };
        }
    }
}
