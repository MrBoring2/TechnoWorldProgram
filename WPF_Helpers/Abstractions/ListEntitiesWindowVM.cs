using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TechoWorld_DataModels_v2.Abstractions;
using WPF_Helpers.Common;
using WPF_Helpers.Models;
using WPF_Helpers.Services;
using WPF_VM_Abstractions;

namespace WPF_Helpers.Abstractions
{
    public abstract class ListEntitiesWindowVM<T, TFilter> : BaseModalWindowVM
     where T : BaseEntity
        where TFilter : FilteredObjects<T>
    {
        private T selectedEntity;
        private Visibility emptyVisibility;
        private int totalFilteredCount;
        private int itemsPerPage;
        private Paginator paginator;
        private SortParameter _selectedSortParameter;
        private int lastPage;
        private string search;

        public ListEntitiesWindowVM(int itemsPerPage)
        {
            ChangePageCommand = new RelayCommand(ChangePage);
            SortOrderChangedCommand = new RelayCommand(SortOrderChanged);
            search = string.Empty;
            ItemsPerPage = itemsPerPage;
            lastPage = 1;
        }

        protected abstract string UrlApi { get; }

        public string Search
        {
            get => search;
            set
            {
                search = value;
                OnPropertyChanged();
                AfterSetSearch(value);
            }
        }
        public SortParameter SelectedSort
        {
            get
            {
                if (_selectedSortParameter is null)
                {
                    _selectedSortParameter = SortParameters.FirstOrDefault();
                }
                return _selectedSortParameter;
            }
            set
            {
                _selectedSortParameter = value;
                OnPropertyChanged();
                AfterSetSelectedSort(value);
            }
        }
        public T SelectedEntity { get => selectedEntity; set { selectedEntity = value; OnPropertyChanged(); } }
        public ObservableCollection<T> EntitiesCollection { get; set; }

        public Paginator Paginator
        {
            get => paginator;
            set
            {
                paginator = value;
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

        public Visibility EmptyVisibility
        {
            get => emptyVisibility;
            set
            {
                emptyVisibility = value;
                OnPropertyChanged();
            }
        }

        public abstract ObservableCollection<SortParameter> SortParameters { get; set; }

        protected abstract object FilterParam { get; }

        public RelayCommand ChangePageCommand { get; set; }
        public RelayCommand SortOrderChangedCommand { get; set; }

        /// <summary>
        /// Происходит после установки значения для <see cref="SelectedSort"/>
        /// </summary>
        /// <param name="value"></param>
        protected virtual async void AfterSetSelectedSort(SortParameter value)
        {
            await GetWithFilter();
        }

        /// <summary>
        /// Происходит после установки значения для <see cref="Search"/>
        /// </summary>
        /// <param name="value"></param>
        protected virtual async void AfterSetSearch(string value)
        {
            await GetWithFilter();
        }

        protected async Task GetWithFilter()
        {
            var request = await ApiService.Instance.GetRequestWithParameter(UrlApi, "jsonFilter", JsonConvert.SerializeObject(FilterParam));
            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<TFilter>(request.Content);
                EntitiesCollection = new ObservableCollection<T>(result.Objects);
                totalFilteredCount = result.TotalFiltered;
                if (Paginator != null)
                {
                    await RefreshCollection();
                }
                else
                {
                    Paginator = new Paginator(5, MaxPage());
                    await RefreshCollection();
                }
            }
        }

        protected async Task RefreshCollection()
        {
            await Task.Run(() =>
            {
                var maxPage = MaxPage();

                Paginator.RefreshPages(maxPage == 0 ? 1 : maxPage);

                if (EntitiesCollection.Count() <= 0)
                {
                    EmptyVisibility = Visibility.Visible;
                    Paginator.SelectedPageNumber = 1;
                }
                else EmptyVisibility = Visibility.Hidden;

                OnPropertyChanged(nameof(EntitiesCollection));
            });
        }

        private int MaxPage()
        {
            return (int)Math.Ceiling((float)totalFilteredCount / (float)ItemsPerPage);
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
                OnPropertyChanged(nameof(EntitiesCollection));
                if (lastPage != Paginator.SelectedPageNumber)
                {
                    lastPage = Paginator.SelectedPageNumber;
                    await GetWithFilter();
                }
                lastPage = Paginator.SelectedPageNumber;
            }
        }

        private async void SortOrderChanged(object obj)
        {
            await GetWithFilter();
        }

    }
}
