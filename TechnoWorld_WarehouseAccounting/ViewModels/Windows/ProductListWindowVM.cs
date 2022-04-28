using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_WarehouseAccounting.Common;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechoWorld_DataModels;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class ProductListWindowVM : BaseModalWindowVM
    {
        private ObservableCollection<string> categories;
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<string> electronicsTypes { get; set; }
        private ObservableCollection<string> forDisplayList { get; set; }
        private int itemsPerPage;
        private string selectedCategory;
        private string selectedType;
        private Paginator paginator;
        private SortParameter selectedSort;
        private Visibility emptyVisibility;
        private Electronic selectedElectronc;
        private bool isCategorySelected;
        private string selectedDisplay;
        private string search;
        public ProductListWindowVM()
        {
            Initialize();
            LoadData();
            SelectProductCommand = new RelayCommand(SelectProduct);
        }
        private void SelectProduct(object obj)
        {
            if (SelectedElectronic != null)
            {
                DialogResult = true;
            }
        }
        public Paginator Paginator { get => paginator; set { paginator = value; OnPropertyChanged(); } }
        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand SortOrderChangedCommand { get; set; }
        public RelayCommand SelectProductCommand { get; set; }
        public ObservableCollection<string> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public ObservableCollection<ElectrnicsType> AllElectronicsTypes { get; set; }
        public ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<string> ForDisplayList { get => forDisplayList; set { forDisplayList = value; OnPropertyChanged(); } }
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
        public ObservableCollection<Electronic> DisplayedElectronics => RefreshElectronics();
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
        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; OnPropertyChanged(); }
        }
        public string SelectedDisplay
        {
            get { return selectedDisplay; }
            set { selectedDisplay = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedElectronics)); }
        }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        public string Search { get => search; set { search = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedElectronics)); } }
        public SortParameter SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedElectronics)); } }
        public Electronic SelectedElectronic { get => selectedElectronc; set { selectedElectronc = value; OnPropertyChanged(); } }
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
                OnPropertyChanged(nameof(DisplayedElectronics));
            }
        }
        public string SelectedType { get => selectedType; set { selectedType = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedElectronics)); } }
        private void Initialize()
        {
            ChangePageCommand = new RelayCommand(ChangePage);
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            IsCategorySelected = false;
            search = string.Empty;
            ItemsPerPage = 15;
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Модель", "Model"),
                new SortParameter("Цена", "Price")
            };
            ForDisplayList = new ObservableCollection<string>
            {
                "Все",
                "Выстевлены на продажу",
                "Сняты с продажи"
            };

            selectedDisplay = ForDisplayList.FirstOrDefault(p => p.Equals("Выстевлены на продажу"));
            selectedSort = SortParameters.FirstOrDefault();

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedDisplay));
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
                Paginator = new Paginator(5, MaxPage());
                OnPropertyChanged(nameof(DisplayedElectronics));
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
        private ObservableCollection<Electronic> RefreshElectronics()
        {
            var maxPage = MaxPage();

            Paginator.RefreshPages(maxPage == 0 ? 1 : maxPage);

            var electronicsList = GetFilteredElectronics(Electronics);

            //Если после фильтрации у нас количество элементов 0, то выводим Пусто
            if (electronicsList.Count() <= 0)
            {
                EmptyVisibility = Visibility.Visible;
                Paginator.SelectedPageNumber = 1;
            }
            else EmptyVisibility = Visibility.Hidden;

            electronicsList = electronicsList.Skip((Paginator.SelectedPageNumber - 1) * itemsPerPage)
               .Take(itemsPerPage).ToList();

            return new ObservableCollection<Electronic>(electronicsList);

            //if (DisplayedElectronics != null)
            //{
            //    DisplayedElectronics.Clear();

            //    list.ForEach(p => DisplayedElectronics.Add(p));
            //}
            //else
            //{
            //    DisplayedElectronics = new ObservableCollection<Electronic>(list);
            //}
        }

        private IEnumerable<Electronic> GetFilteredElectronics(IEnumerable<Electronic> electronics)
        {
            var list = SortElectronics(electronics).ToList();

            if (SelectedDisplay != "Все")
            {
                if (SelectedDisplay == "Выстевлены на продажу")
                {
                    list = list.Where(p => p.IsOfferedForSale == true).ToList();
                }
                else if (SelectedDisplay == "Сняты с продажи")
                {
                    list = list.Where(p => p.IsOfferedForSale == false).ToList();
                }
            }

            if (list.Count > 0)
            {
                list = list.Where(p => p.Model.ToLower().Contains(Search.ToLower())).ToList();
            }

            if (list.Count > 0)
            {
                list = list.Where(p => SelectedCategory != "Все" ? p.Type.Category.Name.Equals(SelectedCategory) : true).ToList();
            }

            if (list.Count > 0)
            {
                list = list.Where(p => SelectedType != "Все" && SelectedType != null ? p.Type.Name.Equals(SelectedType) : true).ToList();
            }



            return list;
        }

        private int MaxPage()
        {
            //Фильтруем наш список по поисковой строке
            var list = GetFilteredElectronics(Electronics);

            return (int)Math.Ceiling((float)list.Count() / (float)ItemsPerPage);
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
            OnPropertyChanged(nameof(DisplayedElectronics));
        }
        private async void ChangePage(object obj)
        {
            if (obj != null)
            {
                if (Paginator != null)
                {

                    if (Convert.ToInt32(obj) == -1)
                    {
                        Paginator.SelectedPageNumber = 1;
                    }
                    else if (Convert.ToInt32(obj) == 1)
                    {
                        Paginator.SelectedPageNumber = MaxPage();
                    }
                }
                else return;
            }
            if (Paginator.DisplayedPagesNumbers.Count > 0)
            {
                await Task.Run(Paginator.RefrashPaginator);
                OnPropertyChanged(nameof(DisplayedElectronics));
            }
        }

    }
}
