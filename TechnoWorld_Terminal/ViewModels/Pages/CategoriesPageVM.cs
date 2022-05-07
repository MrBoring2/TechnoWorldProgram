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

using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Common;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class CategoriesPageVM : PageVMBase
    {
        public delegate void OpenCategoryDelegate(Category category);
        public event OpenCategoryDelegate onOpenCategory;
        private ObservableCollection<Category> categories;
        private Category selectedCategory;
        public CategoriesPageVM()
        {
            LoadCategories();
            OpenCategoryCommand = new RelayCommand(OpenCategory);
        }

        private void OpenCategory(object obj)
        {
            if (obj != null)
            {
                var category = obj as Category;
                onOpenCategory?.Invoke(category);
                PageNavigation.Navigate(typeof(ElectronicsListPageVM));
            }
        }

        public RelayCommand OpenCategoryCommand { get; set; }

        public ObservableCollection<Category> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public Category SelectedCategory { get => selectedCategory; set { selectedCategory = value; OnPropertyChanged(); } }
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
