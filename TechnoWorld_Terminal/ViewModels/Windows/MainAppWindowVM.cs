
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
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Views.Pages;
using System.Windows;
using TechoWorld_DataModels;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using TechnoWorld_Terminal.Models;

namespace TechnoWorld_Terminal.ViewModels.Windows
{
    public class MainAppWindowVM : WindowVMBase
    {
        protected List<PageVMBase> _pageVMs;
        public MainAppWindowVM()
        {
            OpenCartCommand = new RelayCommand(OpenCart);
            ClientService.Instance.Cart.CollectionChanged += Cart_CollectionChanged;
            Initialize();
            Authorize();
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
            ClientService.Instance.HubConnection = new HubConnectionBuilder()
                .WithUrl($"{ApiService.apiUrl}technoWorldHub",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(ClientService.Instance.Token);
                })
                .Build();

            ClientService.Instance.RestClient = new RestClient(ApiService.apiUrl);
            ClientService.Instance.RestClient.Timeout = 20000;
            ClientService.Instance.RestClient.ReadWriteTimeout = 20000;


            WindowLoadedCommand = new RelayCommand(WindowLoaded);

        }

        private void Authorize()
        {
            try
            {
                var response = ApiService.Authorize();
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<TokenModel>(response.Result.Content);
                    ClientService.Instance.SetClient(data.user_name, data.access_token);
                    ClientService.Instance.HubConnection.StartAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void RegisterEvents()
        {
            (PageNavigation.GetPage(typeof(CategoriesPageVM)).DataContext as CategoriesPageVM).onOpenCategory += MainAppWindowVM_onOpenCategory;
        }

        private void MainAppWindowVM_onOpenCategory(Category category)
        {
            (PageNavigation.GetPage(typeof(ElectronicsListPageVM)).DataContext as ElectronicsListPageVM).CurrentCategory = category;
        }


        public RelayCommand WindowLoadedCommand { get; set; }


        private void OpenCart(object obj)
        {
            PageNavigation.Navigate(typeof(CartPageVM));
        }
        private void Exit()
        {
            WindowNavigation.Instance.CloseWindow(this);
        }

        private void WindowLoaded(object obj)
        {
            PageNavigation.Navigate(typeof(CategoriesPageVM));
            RegisterEvents();
        }
    }
}
