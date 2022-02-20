using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class CategoriesPageVM : PageVMBase
    {
        public delegate void OpenCategory(Category category);
        public event OpenCategory onOpenCategory;
        private ObservableCollection<Category> categories;
        private Category selectedCategory;
        public CategoriesPageVM()
        {
            LoadCategories();
            ClientService.Instance.HubConnection.On<string>("SendHello", (message) =>
            {
                MessageBox.Show(message);
            });
            TestCommand = new RelayCommand(Test);
        }

        private async void Test(object obj)
        {
            var response = (RestResponse)await ApiService.GetRequest("api/Categories");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                

            }
        }

        public ObservableCollection<Category> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public Category SelectedCategory { get => selectedCategory; set { selectedCategory = value; OnPropertyChanged(); onOpenCategory?.Invoke(SelectedCategory); PageNavigation.Navigate(typeof(ElectronicsListPageVM)); } }
        public RelayCommand TestCommand { get; set; }
        private async void LoadCategories()
        {
            var response = (RestResponse)await ApiService.GetRequest("api/Categories");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = JsonConvert.DeserializeObject<ObservableCollection<Category>>(response.Content);

            }
        }
    }
}
