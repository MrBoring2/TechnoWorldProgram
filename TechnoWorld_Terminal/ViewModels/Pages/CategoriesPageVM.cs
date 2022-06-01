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
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_VM_Abstractions;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class CategoriesPageVM : BasePageVM
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
                PageNavigation.HidePage(typeof(ElectronicsListPageVM));
                PageNavigation.Navigate(typeof(ElectronicsListPageVM), category.Id);
            }
        }

        public RelayCommand OpenCategoryCommand { get; set; }

        public ObservableCollection<Category> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public Category SelectedCategory { get => selectedCategory; set { selectedCategory = value; OnPropertyChanged(); } }
        private async void LoadCategories()
        {
            var response = (RestResponse)await ApiService.Instance.GetRequest("api/Categories");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<Category>(JsonConvert.DeserializeObject<ObservableCollection<Category>>(response.Content));

            }
        }
    }
}
