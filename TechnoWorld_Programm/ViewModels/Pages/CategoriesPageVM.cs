using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
        public ObservableCollection<Category> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public Category SelectedCategory { get => selectedCategory; set { selectedCategory = value; OnPropertyChanged(); onOpenCategory?.Invoke(SelectedCategory); } }

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
