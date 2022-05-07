using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_API.Models;
using TechnoWorld_WarehouseAccounting.Models;
using TechnoWorld_WarehouseAccounting.Services;
using TechoWorld_DataModels_v2;
using TechoWorld_DataModels_v2.Entities;
using WPF_Helpers.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_Helpers.Services;
using WPF_VM_Abstractions;

namespace TechnoWorld_WarehouseAccounting.ViewModels.Windows
{
    public class ProductListWindowVM : BaseModalWindowVM
    {
        private ObservableCollection<ItemWithTitle<Category>> categories;
        private ObservableCollection<SortParameter> sortParameters;
        private ObservableCollection<ItemWithTitle<ElectrnicsType>> electronicsTypes { get; set; }
        private ObservableCollection<ItemWithTitle<bool?>> forDisplayList { get; set; }
        private int itemsPerPage;
        private int totalFilteredCount;
        private ItemWithTitle<Category> selectedCategory;
        private ItemWithTitle<ElectrnicsType> selectedType;
        private Paginator paginator;
        private SortParameter selectedSort;
        private Visibility emptyVisibility;

        private Electronic selectedElectronc;
        private bool isCategorySelected;
        private ItemWithTitle<bool?> selectedDisplay;
        private string search;
        private int lastPage;
        public ProductListWindowVM()
        {


            Initialize();
            LoadData();
            ApiService.Instance.GetHubConnection.On<string>("UpdateElectronics", async (deliveries) =>
            {
                await GetElectronicsWithFilter();
            });
            //for (int i = 0; i < 10; i++)
            //{
            //    LoadElectronics();
            //}
        }
        public Paginator Paginator { get => paginator; set { paginator = value; OnPropertyChanged(); } }
        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand SortOrderChangedCommand { get; set; }
        public RelayCommand OpenProductWindowCommand { get; set; }
        public RelayCommand OpenEditProductWindowCommand { get; set; }
        public RelayCommand SelectProductCommand { get; set; }
        public ObservableCollection<ItemWithTitle<Category>> Categories { get => categories; set { categories = value; OnPropertyChanged(); } }
        public ObservableCollection<ElectrnicsType> AllElectronicsTypes { get; set; }
        public ObservableCollection<SortParameter> SortParameters { get => sortParameters; set { sortParameters = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<bool?>> ForDisplayList { get => forDisplayList; set { forDisplayList = value; OnPropertyChanged(); } }
        public ObservableCollection<ItemWithTitle<ElectrnicsType>> ElectronicsTypes
        {
            get => electronicsTypes;
            set
            {
                electronicsTypes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Electronic> Electronics { get; set; }
        public ObservableCollection<Electronic> DisplayedElectronics => Electronics;
        public bool IsCategorySelected
        {
            get => isCategorySelected;
            set
            {
                isCategorySelected = value;

                if (isCategorySelected == true)
                {

                    ElectronicsTypes = new ObservableCollection<ItemWithTitle<ElectrnicsType>>(AllElectronicsTypes.Where(p => p.Category.Name == SelectedCategory.Item.Name).Select(p => new ItemWithTitle<ElectrnicsType>(p, p.Name)));
                    ElectronicsTypes.Insert(0, new ItemWithTitle<ElectrnicsType>(null, "Все"));
                    selectedType = ElectronicsTypes.FirstOrDefault();
                    OnPropertyChanged(nameof(SelectedType));
                }
                else
                {
                    ElectronicsTypes?.Clear();
                    selectedType = new ItemWithTitle<ElectrnicsType>(null, "Все");
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
        public ItemWithTitle<bool?> SelectedDisplay
        {
            get { return selectedDisplay; }
            set { selectedDisplay = value; OnPropertyChanged(); GetElectronicsWithFilter(); }
        }
        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }
        public string Search { get => search; set { search = value; OnPropertyChanged(); GetElectronicsWithFilter(); } }
        public SortParameter SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(); GetElectronicsWithFilter(); } }
        public Electronic SelectedElectronic { get => selectedElectronc; set { selectedElectronc = value; OnPropertyChanged(); } }
        public ItemWithTitle<Category> SelectedCategory
        {
            get => selectedCategory;
            set
            {
                selectedCategory = value;
                if (selectedCategory.Title != "Все")
                {
                    IsCategorySelected = true;
                }
                else
                {
                    IsCategorySelected = false;
                }
                OnPropertyChanged();
                GetElectronicsWithFilter();
            }
        }
        public ItemWithTitle<ElectrnicsType> SelectedType
        {
            get => selectedType;
            set
            {
                selectedType = value;
                OnPropertyChanged();
                GetElectronicsWithFilter();
            }
        }
        private void Initialize()
        {
            ChangePageCommand = new RelayCommand(ChangePage);
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            SelectProductCommand = new RelayCommand(SelectProduct);
            IsCategorySelected = false;
            search = string.Empty;
            ItemsPerPage = 15;
            lastPage = 1;
            SortParameters = new ObservableCollection<SortParameter>
            {
                new SortParameter("Модель", "Model"),
                new SortParameter("Цена продажи", "SalePrice"),
                new SortParameter("Цена закупки", "PurchasePrice")
            };
            ForDisplayList = new ObservableCollection<ItemWithTitle<bool?>>
            {
                new ItemWithTitle<bool?>(null, "Все"),
                new ItemWithTitle<bool?>(true, "Выстевлены на продажу"),
                new ItemWithTitle<bool?>(false, "Сняты с продажи")
            };

            selectedDisplay = ForDisplayList.FirstOrDefault(p => p.Item == true);
            selectedSort = SortParameters.FirstOrDefault();

            OnPropertyChanged(nameof(Search));
            OnPropertyChanged(nameof(SelectedDisplay));
            OnPropertyChanged(nameof(SelectedSort));
        }

        private void SelectProduct(object obj)
        {
            if (SelectedElectronic != null)
            {
                DialogResult = true;
            }
        }

        private async void LoadData()
        {
            await LoadCategories();
            await LoadTypes();
            await LoadElectronics();
        }
        private async Task LoadElectronics()
        {
            await GetElectronicsWithFilter();
            //OnPropertyChanged(nameof(DisplayedElectronics));
        }

        private async Task GetElectronicsWithFilter()
        {
            var request = await ApiService.Instance.GetRequestWithParameter("api/Electronics/Filter", "jsonFilter", JsonConvert.SerializeObject(
                new
                {
                    search = Search,
                    categoryId = SelectedCategory.Item == null ? 0 : SelectedCategory.Item.Id,
                    electronicsTypeId = SelectedType.Item == null || SelectedType == null ? 0 : SelectedType.Item.TypeId,
                    sortParameter = SelectedSort.Property,
                    isAscending = SelectedSort.IsAcsending,
                    isOfferedForSale = SelectedDisplay.Item,
                    currentPage = Paginator == null ? 1 : Paginator.SelectedPageNumber,
                    itemsPerPage = ItemsPerPage
                }));
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<FilteredElectronic>(request.Content);
                Electronics = new ObservableCollection<Electronic>(result.Objects);
                totalFilteredCount = result.TotalFiltered;
                if (Paginator != null)
                {
                    await RefreshElectronics();
                }
                else
                {
                    Paginator = new Paginator(5, MaxPage());
                    await RefreshElectronics();
                }
                //dd.Add(JsonConvert.DeserializeObject<List<Electronic>>(request.Content));
            }

        }

        private async Task LoadCategories()
        {
            var request = await ApiService.Instance.GetRequest("api/Categories");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Categories = new ObservableCollection<ItemWithTitle<Category>>(JsonConvert.DeserializeObject<List<Category>>(request.Content).Select(p => new ItemWithTitle<Category>(p, p.Name)));
                Categories.Insert(0, new ItemWithTitle<Category>(null, "Все"));
                selectedCategory = Categories.FirstOrDefault();
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }
        private async Task LoadTypes()
        {
            var request = await ApiService.Instance.GetRequest("api/ElectrnicsTypes/All");
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                AllElectronicsTypes = new ObservableCollection<ElectrnicsType>(JsonConvert.DeserializeObject<List<ElectrnicsType>>(request.Content));
                selectedType = new ItemWithTitle<ElectrnicsType>(null, "Все");
            }
        }
        private async Task RefreshElectronics()
        {
            await Task.Run(() =>
            {
                var maxPage = MaxPage();

                Paginator.RefreshPages(maxPage == 0 ? 1 : maxPage);

                // var electronicsList = GetFilteredElectronics(Electronics);

                //Если после фильтрации у нас количество элементов 0, то выводим Пусто
                if (Electronics.Count() <= 0)
                {
                    EmptyVisibility = Visibility.Visible;
                    Paginator.SelectedPageNumber = 1;
                }
                else EmptyVisibility = Visibility.Hidden;

                //electronicsList = electronicsList.Skip((Paginator.SelectedPageNumber - 1) * itemsPerPage)
                //   .Take(itemsPerPage).ToList();

                OnPropertyChanged(nameof(DisplayedElectronics));
            });

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


        //private IEnumerable<Electronic> GetFilteredElectronics(IEnumerable<Electronic> electronics)
        //{
        //    var list = SortElectronics(electronics).ToList();

        //    if (SelectedDisplay.Title != "Все")
        //    {
        //        if (SelectedDisplay.Item == true)
        //        {
        //            list = list.Where(p => p.IsOfferedForSale == true).ToList();
        //        }
        //        else if (SelectedDisplay.Item == false)
        //        {
        //            list = list.Where(p => p.IsOfferedForSale == false).ToList();
        //        }
        //    }

        //    if (list.Count > 0)
        //    {
        //        list = list.Where(p => p.Model.ToLower().Contains(Search.ToLower())).ToList();
        //    }

        //    if (list.Count > 0)
        //    {
        //        list = list.Where(p => SelectedCategory.Item != null ? p.Type.Category.Name.Equals(SelectedCategory) : true).ToList();
        //    }

        //    if (list.Count > 0)
        //    {
        //        list = list.Where(p => SelectedType.Item != null && SelectedType != null ? p.Type.Name.Equals(SelectedType) : true).ToList();
        //    }



        //    return list;
        //}

        private int MaxPage()
        {
            //Фильтруем наш список по поисковой строке
            //var list = GetFilteredElectronics(Electronics);

            return (int)Math.Ceiling((float)totalFilteredCount / (float)ItemsPerPage);
        }

        //private IEnumerable<Electronic> SortElectronics(IEnumerable<Electronic> electronics)
        //{
        //    if (SelectedSort.IsAcsending)
        //    {
        //        return electronics.OrderBy(p => p.GetProperty(SelectedSort.Property));
        //    }
        //    else
        //    {
        //        return electronics.OrderByDescending(p => p.GetProperty(SelectedSort.Property));
        //    }
        //}
        private async void SortOrderChanged(object obj)
        {
            //
            await GetElectronicsWithFilter();
        }
        private async void ChangePage(object obj)
        {
            if (obj != null)
            {
                if (Paginator != null)
                {

                    if (Convert.ToInt32(obj) == -1)
                    {
                        Paginator.ChangePage(1);
                    }
                    else if (Convert.ToInt32(obj) == 1)
                    {
                        Paginator.ChangePage(MaxPage());
                    }
                }
                else return;
            }
            if (Paginator.DisplayedPagesNumbers.Count > 0)
            {
                await Task.Run(Paginator.RefrashPaginator);
                OnPropertyChanged(nameof(DisplayedElectronics));
                if (lastPage != Paginator.SelectedPageNumber)
                {
                    lastPage = Paginator.SelectedPageNumber;
                    await GetElectronicsWithFilter();
                }
                lastPage = Paginator.SelectedPageNumber;
            }
        }
    }
}
