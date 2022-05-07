
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
            LoginCommand = new RelayCommand(Authorize);
        }
        public string Login
        {
            get => login;
            set { login = value.Length >= 30 ? login : value; OnPropertyChanged(); }
        }
        public string Password { get => password; set { password = value.Length >= 30 ? password : value; OnPropertyChanged(); } }
        public bool IsEnabled { get => isEnabled; set { isEnabled = value; OnPropertyChanged(); } }
        public RelayCommand LoginCommand { get; set; }
        private void Initialize()
        {
            ClientService.Instance.HubConnection = new HubConnectionBuilder()
                .WithUrl($"{ApiService.apiUrl}technoWorldHub",

                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(ClientService.Instance.Token);
                })
                .Build();
            ClientService.Instance.HubConnection.Closed += HubConnection_Closed;

            ClientService.Instance.RestClient = new RestClient(ApiService.apiUrl);
            ClientService.Instance.RestClient.Timeout = 20000;
            ClientService.Instance.RestClient.ReadWriteTimeout = 20000;
        }

        private Task HubConnection_Closed(Exception arg)
        {
            return App.Current.Dispatcher.InvokeAsync(() =>
            {
                CustomMessageBox.Show("Потеряно соединение с сервером!", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                WindowNavigation.Instance.OpenWindow(new LoginWindowVM());
                PageNavigation.Instance.ClearCreatedPages();
                WindowNavigation.Instance.CloseWindows();

            }).Task;
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
                var response = await ApiService.Authorize(Login, Password);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<TokenModel>(response.Content);
                    ClientService.Instance.SetClient(data.user_name, data.full_name, data.role_id, data.user_id, data.post, data.access_token);
                    ClientService.Instance.HubConnection.StartAsync();

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
