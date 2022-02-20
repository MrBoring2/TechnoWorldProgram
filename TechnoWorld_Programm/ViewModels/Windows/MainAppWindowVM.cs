
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
    public class MainAppWindowVM : WindowWithPagesVMBase
    {
        protected List<PageVMBase> _pageVMs;
        private ItemMenu selectedMenuItem;
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
            (GetPageInstance(typeof(CategoriesPageVM)) as CategoriesPageVM).onOpenCategory += MainAppWindowVM_onOpenCategory;
        }

        private void MainAppWindowVM_onOpenCategory(Category category)
        {
            (GetPageInstance(typeof(ElectronicsListPageVM)) as ElectronicsListPageVM).CurrentCategory = category;
            SwitchPage(GetPageInstance(typeof(ElectronicsListPageVM)));
        }

        public List<PageVMBase> PageVMs
        {
            get
            {
                if (_pageVMs == null)
                    _pageVMs = new List<PageVMBase>();
                return _pageVMs;
            }
        }
        public ItemMenu SelectedMenuItem
        {
            get
            {
                return selectedMenuItem;
            }
            set
            {
                selectedMenuItem = value;
                if (SelectedMenuItem.Title.Equals(OptionalMenuItems.FirstOrDefault().Title))
                {
                    Exit();
                }
                SwitchPage(SelectedMenuItem.TargetPageVM);
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ItemMenu> MenuItems { get; set; }
        public ObservableCollection<ItemMenu> OptionalMenuItems { get; set; }
        private void RegisterPages()
        {
            RegisterPageWithVM<ElectronicsListPageVM, ElectronicsListPage>();
            RegisterPageWithVM<CartPageVM, CartPage>();
            RegisterPageWithVM<CategoriesPageVM, CategoriesPage>();
        }
        private void Exit()
        {
            WindowNavigation.Instance.CloseWindow(this);
        }

    }
}
