
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
using TechoWorld_DataModels_v2.Models;
using SweetAlertSharp;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using TechnoWorld_Notification;
using TechnoWorld_Notification.Enums;

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

                    return;
                }

                IsEnabled = false;
                var response = await ApiService.Instance.Authorize(Login, Password);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<AuthResponseModel>(response.Content);
                    ClientService.Instance.SetClient(data.user_name, data.full_name, data.role_id, data.user_id, data.post);

                    await ApiService.Instance.GetHubConnection.StartAsync();

                    WindowNavigation.Instance.OpenAndHideWindow(this, new MainAppWindowVM());
                    MaterialNotification.Show("Оповещение", $"Добро пожаловать, {data.full_name}.", MaterialNotificationButton.Ok, MaterialNotificationImage.Information);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    MaterialNotification.Show("Ошибка", JsonConvert.DeserializeObject<string>(response.Content), MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    IsEnabled = true;
                }
                else
                {
                    MaterialNotification.Show("Критическая ошибка.", "Потярено соединение с сервером!", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                    IsEnabled = true;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
