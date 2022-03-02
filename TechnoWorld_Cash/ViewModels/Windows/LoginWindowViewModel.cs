﻿
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
using TechnoWorld_Terminal.Common;
using System.Security;
using TechnoWorld_Cash.Services;
using TechnoWorld_Cash.Views.Windows;

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
            ClientService.Instance.HubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:29320/technoWorldHub",

                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(ClientService.Instance.Token);
                })
                .Build();

            ClientService.Instance.RestClient = new RestClient(ApiService.apiUrl);
        }
        private async void Authorize(object obj)
        {
            try
            {
                IsEnabled = false;
                var response = await ApiService.Authorize(Login, Password);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<TokenModel>(response.Content);
                    ClientService.Instance.SetClient(data.user_name, data.full_name, data.role_id, data.user_id, data.access_token);
                    ClientService.Instance.HubConnection.StartAsync();

                    CustomMessageBox.Show($"Добро пожаловать, {data.full_name}", "Оповещение", MessageBoxButton.OK, MessageBoxImage.Information);
                    WindowNavigation.Instance.OpenAndHideWindow(this, new CashWindowViewModel());
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