
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
            try
            {
                Initialize();
                Authorize();
                RegisterPages();
                RegisterEvents();
                SwitchPage(GetPageInstance(typeof(CategoriesPageVM)));
            }
            catch (Exception ex)
            {

            }

        }

        private void Initialize()
        {
            ClientService.Instance.HubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:29320/technoWorldHub",
                options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(ClientService.Token);
                })
                .Build();

            ClientService.Instance.RestClient = new RestClient(ApiService.apiUrl);
<<<<<<< HEAD
=======

            WindowLoadedCommand = new RelayCommand(WindowLoaded);
>>>>>>> 8b41cfe17bc9ed46db224f2fdcfc9c7fe2ebed86
        }

        private async void Authorize()
        {
            try
            {
                var response = ApiService.Authorize();
                if (response.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var data = JsonConvert.DeserializeObject<TokenModel>(response.Result.Content);
                    ClientService.Token = data.access_token;
                    //ClientService.Instance.HubConnection.StartAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void RegisterEvents()
        {
            (PageNavigation.GetPage(typeof(CategoriesPageVM)).DataContext as CategoriesPageVM).onOpenCategory += MainAppWindowVM_onOpenCategory;        }

        private void MainAppWindowVM_onOpenCategory(Category category)
        {
            (PageNavigation.GetPage(typeof(ElectronicsListPageVM)).DataContext as ElectronicsListPageVM).CurrentCategory = category;
        }

        //private void MainAppWindowVM_onOpenCategory(Category category)
        //{
        //    (GetPageInstance(typeof(ElectronicsListPageVM)) as ElectronicsListPageVM).CurrentCategory = category;
        //    //SwitchPage(GetPageInstance(typeof(ElectronicsListPageVM)));
        //}
        public RelayCommand WindowLoadedCommand { get; set; }
        //public List<PageVMBase> PageVMs
        //{
        //    get
        //    {
        //        if (_pageVMs == null)
        //            _pageVMs = new List<PageVMBase>();
        //        return _pageVMs;
        //    }
        //}
     
        private void Exit()
        {
            WindowNavigation.Instance.CloseWindow(this);
        }
<<<<<<< HEAD

=======
        private void WindowLoaded(object obj)
        {
            PageNavigation.Navigate(typeof(CategoriesPageVM));
            RegisterEvents();
        }
>>>>>>> 8b41cfe17bc9ed46db224f2fdcfc9c7fe2ebed86
    }
}
