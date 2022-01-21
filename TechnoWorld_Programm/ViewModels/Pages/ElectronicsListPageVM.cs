using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using TechnoWorld_Terminal.Models;
using TechnoWorld_Programm.POCO_Models;
using TechnoWorld_Terminal.Services;

namespace TechnoWorld_Terminal.ViewModels.Pages
{
    public class ElectronicsListPageVM : PageVMBase
    {
        #region Fields
        private ObservableCollection<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;
        private ObservableCollection<Electronic> electronics;
        private ObservableCollection<Electronic> displayedElectronics;

        private ObservableCollection<ElectrnicsType> types;
        private ObservableCollection<Manufacturer> manufacturers;
        private SortParameter selectedSort;
        private Electronic selectedElectronic;
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
        }

        #region Properties
        public ObservableCollection<int> DisplayedPagesNumbers { get => displayedPagesNumbers; set { displayedPagesNumbers = value; OnPropertyChanged(); } }
        public ObservableCollection<int> PagesNumbers { get => pagesNumbers; set { pagesNumbers = value; OnPropertyChanged(); } }
        public ObservableCollection<Electronic> Electronics { get => electronics; set { electronics = value; OnPropertyChanged(); } }
        public ObservableCollection<Electronic> DisplayedElectronics { get => displayedElectronics; set { displayedElectronics = value; OnPropertyChanged(); } }
        public ObservableCollection<ElectrnicsType> Types
        {
            get => types;
            set
            {
                types = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Manufacturer> Manufacturers
        {
            get => manufacturers;
            set
            {
                manufacturers = value;
                OnPropertyChanged();
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
            set { selectedPageNumber = value; OnPropertyChanged(); }
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

        private async void InitializeFields()
        {
            currentPage = 0;
            itemsPerPage = 1;
            maxDisplayedPages = 5;
            selectedPageNumber = 1;
            search = string.Empty;
            EmptyVisibility = Visibility.Hidden;

            await Task.Run(() =>
            {
                LoadElectronics();
                LoadTypes();
                LoadManufacturers();
                LoadSortParams();
            });

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
            var response = (RestResponse)await ApiService.GetRequest("api/ElectrnicsTypes");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Types = JsonSerializer.Deserialize<ObservableCollection<ElectrnicsType>>(response.Content);
            }
        }

        private async void LoadManufacturers()
        {
            var response = (RestResponse)await ApiService.GetRequest("api/Manufacturers");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Manufacturers = JsonSerializer.Deserialize<ObservableCollection<Manufacturer>>(response.Content);
            }
        }

        private async void LoadElectronics()
        {
            var response = (RestResponse)await ApiService.GetRequest("api/Electronics");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Electronics = JsonSerializer.Deserialize<ObservableCollection<Electronic>>(response.Content);
                DisplayedElectronics = Electronics;
                LoadPages();
                DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers.Take(maxDisplayedPages));
            }
        }
        private void LoadPages()
        {
            PagesNumbers = new ObservableCollection<int>();
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
        private void RefreshElectronics()
        {
            var list = SortElectronics(Electronics).ToList();
            list = list.Where(p => p.Model.Contains(Search)).ToList();

            var selectedTypes = Types.Where(p => p.IsSelected);

            if (selectedTypes.Count() > 0)
            {
                foreach (var item in list.ToArray())
                {
                    if (selectedTypes.FirstOrDefault(p => p.Name.Equals(item.Type.Name)) == null)
                        list.Remove(item);
                    else continue;
                }
            }

            var selectedManufacturers = Manufacturers.Where(p => p.IsSelected);

            if (selectedManufacturers.Count() > 0)
            {
                foreach (var item in list.ToArray())
                {
                    if (selectedManufacturers.FirstOrDefault(p => p.Name.Equals(item.Manufacturer.Name)) == null)
                        list.Remove(item);
                    else continue;
                }
            }

            if (MaxPrice > 0)
                list = list.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice).ToList();

            list = list.Skip((SelectedPageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage).ToList();

            DisplayedElectronics = new ObservableCollection<Electronic>(list);
            RefreshPages();

            if (DisplayedElectronics.Count <= 0)
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
                foreach (var item in list.ToArray())
                {
                    if (selectedTypes.FirstOrDefault(p => p.Name.Equals(item.Type.Name)) == null)
                        list.Remove(item);
                    else continue;
                }
            }
            var selectedManufacturers = Manufacturers.Where(p => p.IsSelected);

            if (selectedManufacturers.Count() > 0)
            {
                foreach (var item in list.ToArray())
                {
                    if (selectedManufacturers.FirstOrDefault(p => p.Name.Equals(item.Manufacturer.Name)) == null)
                    {
                        list.Remove(item);
                    }
                    else
                    {
                        continue;
                    }
                }
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
