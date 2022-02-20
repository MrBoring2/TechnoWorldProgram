using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TechnoWorld_Terminal.Common;
using TechnoWorld_Terminal.Models;
using TechnoWorld_Terminal.Services;
using TechoWorld_DataModels;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class ElectronicsListPageVM : PageVMBase
    {
        #region Fields
        private List<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;
        private List<Electronic> electronics;
        private IEnumerable<Electronic> displayedElectronics;

        private List<ElectrnicsType> types;
        private List<Manufacturer> manufacturers;
        private SortParameter selectedSort;
        private Electronic selectedElectronic;
        private Category category;
        private Visibility emptyVisibility;
        private string search;
        private int currentPage;
        private bool orderByDescening;
        private int itemsPerPage;
        private int maxDisplayedPages;
        private int totalPages;
        private int selectedPageNumber;
        private int minPrice;
        private int maxPrice;
        #endregion

        public ElectronicsListPageVM()
        {
            InitializeFields();
            // OnPropertyChanged(nameof(Manufacturers));
            // OnPropertyChanged(nameof(DisplayedElectronics));
            // OnPropertyChanged(nameof(DisplayedPagesNumbers));
            //OnPropertyChanged(nameof(SortParameters));
        }

        #region Properties
        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand ToFirstPageCommand { get; set; }
        public RelayCommand ToLastPageCommand { get; set; }
        public ObservableCollection<int> DisplayedPagesNumbers { get => displayedPagesNumbers; set { displayedPagesNumbers = value; OnPropertyChanged(); } }
        public List<int> PagesNumbers { get => pagesNumbers; set { pagesNumbers = value; OnPropertyChanged(); } }
        public List<Electronic> Electronics { get => electronics; set { electronics = value; OnPropertyChanged(); } }
        public IEnumerable<Electronic> DisplayedElectronics { get => displayedElectronics; set { displayedElectronics = value; OnPropertyChanged(); } }
        public List<ElectrnicsType> Types
        {
            get => types;
            set
            {
                types = value;
                OnPropertyChanged();
            }
        }
        public List<Manufacturer> Manufacturers
        {
            get => manufacturers;
            set
            {
                manufacturers = value;
                OnPropertyChanged();
            }
        }

        public Category CurrentCategory
        {
            get => category;
            set
            {
                category = value;
                OnPropertyChanged();
                LoadData();
            }
        }
        public List<SortParameter> SortParameters { get; set; }
        public SortParameter SelectedSort { get => selectedSort; set { selectedSort = value; OnPropertyChanged(); RefreshElectronics(); } }

        public Electronic SelectedElectronic { get => selectedElectronic; set { selectedElectronic = value; OnPropertyChanged(); } }

        public Visibility EmptyVisibility { get => emptyVisibility; set { emptyVisibility = value; OnPropertyChanged(); } }

        public string Search
        {
            get => search;
            set
            {
                search = value;
                OnPropertyChanged();
                RefreshElectronics();
            }
        }
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; OnPropertyChanged(); }
        }

        public int ItemsPerPage
        {
            get { return itemsPerPage; }
            set { itemsPerPage = value; OnPropertyChanged(); }
        }
        public int MaxDisplayedPages
        {
            get { return maxDisplayedPages; }
            set { maxDisplayedPages = value; OnPropertyChanged(); }
        }
        public int TotalPages
        {
            get { return totalPages; }
            set { totalPages = value; OnPropertyChanged(); }
        }
        public int SelectedPageNumber
        {
            get { return selectedPageNumber; }
            set
            {
                selectedPageNumber = value;

                OnPropertyChanged();

                //OnPropertyChanged(nameof(DisplayedPagesNumbers);
            }
        }
        public int MinPrice
        {
            get { return minPrice; }
            set { minPrice = value; OnPropertyChanged(); }
        }
        public int MaxPrice
        {
            get { return maxPrice; }
            set { maxPrice = value; OnPropertyChanged(); }
        }
        public bool OrderByDescening { get => orderByDescening; set { orderByDescening = value; OnPropertyChanged(); } }
        #endregion

        private void InitializeFields()
        {
            currentPage = 0;
            itemsPerPage = 5;
            maxDisplayedPages = 5;
            selectedPageNumber = 1;
            search = string.Empty;
            ChangePageCommand = new RelayCommand(ChangePage);
            ToFirstPageCommand = new RelayCommand(ToFirstPage);
            ToLastPageCommand = new RelayCommand(ToLastPage);
            EmptyVisibility = Visibility.Hidden;
        }

        private async void ToLastPage(object obj)
        {
            SelectedPageNumber = PagesNumbers.LastOrDefault();

            await Task.Run(RefrashPaginator);
            OnPropertyChanged(nameof(SelectedPageNumber));
        }

        private async void ToFirstPage(object obj)
        {
            SelectedPageNumber = PagesNumbers.FirstOrDefault();

            await Task.Run(RefrashPaginator);
            OnPropertyChanged(nameof(SelectedPageNumber));
        }

        private async void ChangePage(object obj)
        {
            if (obj != null)
            {
                if (Convert.ToInt32(obj) == 0)
                {
                    SelectedPageNumber = 1;
                }
                else if (Convert.ToInt32(obj) == 1)
                {
                    SelectedPageNumber = MaxPage();
                }
            }
            await Task.Run(RefrashPaginator);
            await Task.Run(RefreshElectronics);
        }

        private async void LoadData()
        {
            await Task.Run(() => LoadManufacturers());
            await Task.Run(() => LoadTypes());
            await Task.Run(() => LoadSortParams());
            await Task.Run(() => LoadElectronics());
            OnPropertyChanged(nameof(SortParameters));
            OnPropertyChanged(nameof(SelectedSort));
        }
        private async void RealoadElectronics()
        {
            await Task.Run(LoadElectronics);
        }


        private void LoadSortParams()
        {
            SortParameters = new List<SortParameter>
            {
                new SortParameter("Модель", "Model"),
                new SortParameter("Цена", "Price"),
                new SortParameter("Производитель", "ManufacturerName")
            };
            selectedSort = SortParameters.FirstOrDefault();
        }
        private async void LoadTypes()
        {
            var response = (RestResponse)await ApiService.GetRequestWithParameter("api/ElectrnicsTypes", "categoryId", CurrentCategory.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Types = JsonConvert.DeserializeObject<List<ElectrnicsType>>(response.Content);
                foreach (var item in Types)
                {
                    item.OnSelectionChanged += Type_OnSelectionChanged1;
                }
            }

        }


        private async void LoadManufacturers()
        {
            var response = (RestResponse)await ApiService.GetRequestWithParameter("api/Manufacturers", "categoryId", CurrentCategory.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Manufacturers = JsonConvert.DeserializeObject<List<Manufacturer>>(response.Content);
                foreach (var item in Manufacturers)
                {
                    item.OnSelectionChanged += Manufacturer_OnSelectionChanged;
                }
            }
        }

        private async void Manufacturer_OnSelectionChanged(object sender, EventArgs e)
        {
            await Task.Run(RefreshElectronics);
        }
        private async void Type_OnSelectionChanged1(object sender, EventArgs e)
        {
            await Task.Run(RefreshElectronics);
        }

        private async void LoadElectronics()
        {
            var response = (RestResponse)await ApiService.GetRequestWithParameter($"api/Electronics", "categoryId", CurrentCategory.Id);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Electronics = JsonConvert.DeserializeObject<List<Electronic>>(response.Content);


                LoadPages();
                DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers.Take(maxDisplayedPages));
            }
        }
        private void LoadPages()
        {
            PagesNumbers = new List<int>();
            var max = MaxPage();
            for (int i = 0; i < max; i++)
            {
                PagesNumbers.Add(i + 1);
            }
        }
        /// <summary>
        /// Обновить пагинатор
        /// </summary>
        private void RefreshPages()
        {
            LoadPages();
            RefrashPaginator();
            if (SelectedPageNumber > PagesNumbers.Count)
            {
               
                SelectedPageNumber = DisplayedPagesNumbers.LastOrDefault();
            }
        }
        /// <summary>
        /// Обновить список электроники согласно с фильтром
        /// </summary>
        private async void RefreshElectronics()
        {
            var list = SortElectronics(Electronics);
            list = list.Where(p => p.Model.Contains(Search));

            var selectedTypes = Types.Where(p => p.IsSelected);

            if (selectedTypes.Count() > 0)
            {
                list = list.Where(p => selectedTypes.Contains(selectedTypes.FirstOrDefault(g => g.TypeId == p.TypeId)));
            }

            var selectedManufacturers = Manufacturers.Where(p => p.IsSelected);

            if (selectedManufacturers.Count() > 0)
            {
                list = list.Where(p => selectedManufacturers.Contains(selectedManufacturers.FirstOrDefault(g => g.ManufacturerId == p.ManufactrurerId)));
            }

            if (MaxPrice > 0)
                list = list.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice);


            //if (DisplayedElectronics != null)
            //{
            //    App.Current.Dispatcher.Invoke(() => DisplayedElectronics.Clear());
            //}
            DisplayedElectronics = list.Skip((SelectedPageNumber - 1) * itemsPerPage)
               .Take(itemsPerPage);

            //list.Clear();
            RefreshPages();

            if (DisplayedElectronics.Count() <= 0)
            {
                EmptyVisibility = Visibility.Visible;
            }
            else EmptyVisibility = Visibility.Hidden;
        }
        /// <summary>
        /// Сортировка списка электроники
        /// </summary>
        /// <param name="electronics"></param>
        /// <returns></returns>
        private IEnumerable<Electronic> SortElectronics(IEnumerable<Electronic> electronics)
        {
            if (SelectedSort.IsDescening)
            {
                electronics = electronics.OrderByDescending(p => p.GetProperty(SelectedSort.Property));
            }
            else
            {
                electronics = electronics.OrderBy(p => p.GetProperty(SelectedSort.Property));
            }
            return electronics;
        }
        /// <summary>
        /// Вычисление максимальной страницы
        /// </summary>
        /// <returns></returns>
        private int MaxPage()
        {
            var list = Electronics
                     .Where(p => p.Model.Contains(Search)).ToList();

            var selectedTypes = Types.Where(p => p.IsSelected);

            if (selectedTypes.Count() > 0)
            {
                list = list.Where(p => selectedTypes.Contains(selectedTypes.FirstOrDefault(g => g.TypeId == p.TypeId))).ToList();
            }
            var selectedManufacturers = new List<Manufacturer>();

            selectedManufacturers = Manufacturers.Where(p => p.IsSelected).ToList();


            if (selectedManufacturers.Count() > 0)
            {
                list = list.Where(p => selectedManufacturers.Contains(selectedManufacturers.FirstOrDefault(g => g.ManufacturerId == p.ManufactrurerId))).ToList();
            }

            if (MaxPrice > 0)
                list = list.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice).ToList();

            return (int)Math.Ceiling((float)list.Count / (float)ItemsPerPage);
        }

        /// <summary>
        /// Перейти на другую страницу
        /// </summary>
        public void RefrashPaginator()
        {
            
            if (SelectedPageNumber <= PageListAvg(DisplayedPagesNumbers))
            {
                DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                    .Take(maxDisplayedPages));
            }
            else
            {
                if (PagesNumbers.Skip(SelectedPageNumber - PageListAvg(DisplayedPagesNumbers)).Count() > maxDisplayedPages)
                    DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                        .Skip(SelectedPageNumber - PageListAvg(DisplayedPagesNumbers))
                        .Take(maxDisplayedPages));

                else
                    DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers
                       .Skip(PagesNumbers.Count - maxDisplayedPages)
                       .Take(maxDisplayedPages));
            }
        }
        /// <summary>
        /// Функция для вычисления среднего количества элементов в последовательности
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        private int PageListAvg(IEnumerable<int> collection)
        {
            return Convert.ToInt32(Math.Ceiling(collection.Count() / (float)2));
        }



    }
}
