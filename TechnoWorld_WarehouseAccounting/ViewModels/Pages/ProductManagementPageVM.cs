using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Pages
{
    public class ProductManagementPageVM : BasePageVM
    {
        private ObservableCollection<string> categories;
        private ObservableCollection<Electronic> displayedElectronics;
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<string> electronicsTypes { get; set; }
        private string selectedCategory;
        private string selectedType;
        private SortParameter selectedSort;

        private bool isCategorySelected;
        private string search;
        public ProductManagementPageVM()
        {
            Initialize();
            LoadData();
        }
        public RelayCommand SortOrderChangedCommand { get; set; }
        public ObservableCollection<string> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public ObservableCollection<ElectrnicsType> AllElectronicsTypes { get; set; }
        public ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<string> ElectronicsTypes
        {
            get => electronicsTypes;
            set
            {
                electronicsTypes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Electronic> Electronics { get; set; }
        public ObservableCollection<Electronic> DisplayedElectronics { get => displayedElectronics; set { displayedElectronics = value; OnPropertyChanged(); } }
        public bool IsCategorySelected
        {
            get => isCategorySelected;
            set
            {
                isCategorySelected = value;

                if (isCategorySelected == true)
                {

                    ElectronicsTypes = new ObservableCollection<string>(AllElectronicsTypes.Where(p => p.Category.Name == SelectedCategory).Select(p => p.Name));
                    ElectronicsTypes.Insert(0, "Все");
                    selectedType = ElectronicsTypes.FirstOrDefault();
                    OnPropertyChanged(nameof(SelectedType));
                }
                else
                {
                    ElectronicsTypes?.Clear();
                }
                OnPropertyChanged();
            }
        }
        public string Search { get => search; set { search = value; OnPropertyChanged(); RefreshElectronics(); } }
        public SortParameter SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(); RefreshElectronics(); } }
        public string SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                if (selectedCategory != "Все")
                {
                    IsCategorySelected = true;
                }
                else
                {
                    IsCategorySelected = false;
                }
                OnPropertyChanged();
                RefreshElectronics();
            }
        }
        public string SelectedType { get => selectedType; set { selectedType = value; OnPropertyChanged(); RefreshElectronics(); } }
        private void Initialize()
        {
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            IsCategorySelected = false;
            search = string.Empty;

            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Модель", "Model"),
                new SortParameter("Цена", "Price")
            };
            selectedSort = SortParameters.FirstOrDefault();

            OnPropertyChanged(nameof(search));
            OnPropertyChanged(nameof(SelectedSort));
        }


        private async void LoadData()
        {
            await LoadCategories();
            await LoadTypes();
            await LoadElectronics();
        }
        private async Task LoadElectronics()
        {
            var request = await ApiService.GetRequest("api/Electronics/All");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Electronics = new ObservableCollection<Electronic>(JsonConvert.DeserializeObject<List<Electronic>>(request.Content));
                RefreshElectronics();
            }
        }
        private async Task LoadCategories()
        {
            var request = await ApiService.GetRequest("api/Categories");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<string>(JsonConvert.DeserializeObject<List<Category>>(request.Content).Select(p => p.Name));
                Categories.Insert(0, "Все");
                selectedCategory = Categories.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        private async Task LoadTypes()
        {
            var request = await ApiService.GetRequest("api/ElectrnicsTypes/All");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AllElectronicsTypes = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(request.Content));
            }
        }
        private void RefreshElectronics()
        {
            var list = SortElectronics(Electronics).ToList();

            list = list.Where(p => p.Model.ToLower().Contains(Search.ToLower())).ToList();

            if (list.Count > 0)
            {
                list = list.Where(p => SelectedCategory != "Все" ? p.Type.Category.Name.Equals(SelectedCategory) : true).ToList();
            }

            if (list.Count > 0)
            {
                list = list.Where(p => SelectedType != "Все" && SelectedType != null ? p.Type.Name.Equals(SelectedType) : true).ToList();
            }

            if (DisplayedElectronics != null)
            {
                DisplayedElectronics.Clear();

                list.ForEach(p => DisplayedElectronics.Add(p));
            }
            else
            {
                DisplayedElectronics = new ObservableCollection<Electronic>(list);
            }
        }
        private IEnumerable<Electronic> SortElectronics(IEnumerable<Electronic> electronics)
        {
            if (SelectedSort.IsAcsending)
            {
                return electronics.OrderBy(p => p.GetProperty(SelectedSort.Property));
            }
            else
            {
                return electronics.OrderByDescending(p => p.GetProperty(SelectedSort.Property));
            }
        }
        private void SortOrderChanged(object obj)
        {
            RefreshElectronics();
        }
    }
}
