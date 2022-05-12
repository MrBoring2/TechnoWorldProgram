
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;

using BNS_Programm.CustomElements;
using System.Collections.ObjectModel;
using MaterialDesignThemes.Wpf;
using TechnoWorld_Terminal.ViewModels.Pages;
using TechnoWorld_Terminal.Services;
using TechnoWorld_Terminal.Views.Pages;
using System.Windows;
using TechoWorld_DataModels_v2;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using TechnoWorld_Terminal.Models;
using WPF_Helpers.Common;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_VM_Abstractions;
using TechoWorld_DataModels_v2.Models;
using TechnoWorld_Notification;
using TechnoWorld_Notification.Enums;

namespace TechnoWorld_Terminal.ViewModels.Windows
{
    public class MainAppWindowVM : BaseWindowVM
    {
        public MainAppWindowVM()
        {
            OpenCartCommand = new RelayCommand(OpenCart);
            ClientService.Instance.Cart.CollectionChanged += Cart_CollectionChanged;
            Initialize();
           
            OnPropertyChanged(nameof(ItemsInCart));

        }

        private void Cart_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ItemsInCart));
        }

        public RelayCommand OpenCartCommand { get; set; }
        public int ItemsInCart => ClientService.Instance.Cart.Count;
        private void Initialize()
        {

            WindowLoadedCommand = new RelayCommand(WindowLoaded);

        }
        private async Task Authorize()
        {
            var response = await ApiService.Instance.AuthorizeTerminal();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var data = JsonConvert.DeserializeObject<AuthResponseModel>(response.Content);
                ClientService.Instance.SetClient(data.user_name);
                await ApiService.Instance.GetHubConnection.StartAsync();
                PageNavigation.Navigate(typeof(CategoriesPageVM));
            }
            else
            {
                MaterialNotification.Show("Ошибка", "Не удалось подключиться к серверу", MaterialNotificationButton.Ok, MaterialNotificationImage.Error);
                ApiService.Instance.ShutDownService();
            }
            await Task.Delay(5000).ContinueWith(task => Reconnect());
        }

        public RelayCommand WindowLoadedCommand { get; set; }


        private void OpenCart(object obj)
        {
            if (ApiService.Instance.GetHubConnection.State == HubConnectionState.Connected)
            {
                PageNavigation.Navigate(typeof(CartPageVM));
            }
        }

        private void Exit()
        {
            WindowNavigation.Instance.CloseWindow(this);
        }

        private async void Reconnect()
        {
            await Application.Current.Dispatcher.InvokeAsync(Authorize);
        }

        private async void WindowLoaded(object obj)
        {
            await Authorize();
            
            //RegisterEvents();
        }
    }
}
