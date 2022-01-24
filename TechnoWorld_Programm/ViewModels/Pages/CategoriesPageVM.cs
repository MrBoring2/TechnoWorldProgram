using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Programm.POCO_Models;
using TechnoWorld_Terminal.POCO_Models;
using TechnoWorld_Terminal.Services;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class CategoriesPageVM : PageVMBase
    {
        private ObservableCollection<Category> categories;
        public CategoriesPageVM()
        {
            LoadCategories();
        }
        public ObservableCollection<Category> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
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
