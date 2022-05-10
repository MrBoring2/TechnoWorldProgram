
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Terminal.Services;
using Microsoft.AspNetCore.SignalR.Client;
using RestSharp;
using Newtonsoft.Json;
using TechnoWorld_Terminal.Models;
using System.Windows;
using System.Security;
using TechnoWorld_Cash.Services;
using TechnoWorld_Cash.Views.Windows;
using WPF_Helpers.Common;
using TechnoWorld_Cash.ViewModels.Pages;
using WPF_VM_Abstractions;
using WPF_Helpers.Abstractions;
using TechoWorld_DataModels_v2.Models;
using TechnoWorld_Notification;
using TechnoWorld_Notification.Enums;

namespace TechnoWorld_Cash.ViewModels.Windows
{
    public class LoginWindowViewModel : BaseWindowVM
    {
        private string login;
        private string password;
        private bool isEnabled = true;
        public LoginWindowViewModel()
        {
            Initialize();
            LoginCommand = new RelayCommand(Authorize);
        }
        public string Login { get => login; set { login = value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        public bool IsEnabled { get => isEnabled; set { isEnabled = value; OnPropertyChanged(); } }
        public RelayCommand LoginCommand { get; set; }
        private void Initialize()
        {

        }

        /// <summary>
        /// Авторизация
        /// </summary>
        /// <param name="obj"></param>
        private async void Authorize(object obj)
        {
            try
            {
                IsEnabled = false;
                var response = await ApiService.Instance.Authorize(Login, Password);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<AuthResponseModel>(response.Content);
                    ClientService.Instance.SetClient(data.user_name, data.full_name, data.role_id, data.user_id, data.post);
                    await ApiService.Instance.GetHubConnection.StartAsync();
            
           
                    WindowNavigation.Instance.OpenAndHideWindow(this, new MainAppWindowVM());
                }
                else
                {
                    CustomMessageBox.Show(JsonConvert.DeserializeObject<string>(response.Content), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsEnabled = true;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
