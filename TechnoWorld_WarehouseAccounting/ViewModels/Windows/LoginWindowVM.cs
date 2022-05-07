
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Services;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using Newtonsoft.Json;
using TechnoWorld_WarehouseAccounting.Models;
using System.Windows;
using System.Security;
using TechnoWorld_WarehouseAccounting.Views.Windows;
using WPF_VM_Abstractions;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class LoginWindowVM : BaseWindowVM
    {
        private string login;
        private string password;
        private bool isEnabled = true;
        public LoginWindowVM()
        {
            Initialize();
        }
        public string Login { get => login; set { login = value.Length >= 30 ? login : value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value.Length >= 30 ? password : value; OnPropertyChanged(); } }
        public bool IsEnabled { get => isEnabled; set { isEnabled = value; OnPropertyChanged(); } }
        public RelayCommand LoginCommand { get; set; }
        private void Initialize()
        {
            LoginCommand = new RelayCommand(Authorize);
        }
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="obj"></param>
        private async void Authorize(object obj)
        {
            try
            {
                if (Login.Length >= 30 || Password.Length >= 30)
                {
                    CustomMessageBox.Show("Слишком большие данные", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                IsEnabled = false;
                var response = await ApiService.Instance.Authorize(Login, Password);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<AuthResponseModel>(response.Content);
                    ClientService.Instance.SetClient(data.user_name, data.full_name, data.role_id, data.user_id, data.post);

                    await ApiService.Instance.GetHubConnection.StartAsync();

                    CustomMessageBox.Show($"Добро пожаловать, {data.full_name}", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                    WindowNavigation.Instance.OpenAndHideWindow(this, new MainAppWindowVM());
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    CustomMessageBox.Show(JsonConvert.DeserializeObject<string>(response.Content), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsEnabled = true;
                }
                else
                {
                    CustomMessageBox.Show("Не удаётся соединиться с сервером!", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsEnabled = true;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
