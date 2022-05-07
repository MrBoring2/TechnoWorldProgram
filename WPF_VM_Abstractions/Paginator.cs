using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_VM_Abstractions
{
    public class Paginator : INotifyPropertyChanged
    {
        private int maxDisplayedPages;
        private int selectedPageNumber;
        private bool isSelectionEnabled = true;
        private ObservableCollection<int> pagesNumbers;
        private ObservableCollection<int> displayedPagesNumbers;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public Paginator(int _maxDisplayedPages, int maxPage)
        {
            maxDisplayedPages = _maxDisplayedPages;
            LoadPages(maxPage);
            DisplayedPagesNumbers = new ObservableCollection<int>(PagesNumbers.Take(maxDisplayedPages));
            SelectedPageNumber = DisplayedPagesNumbers.FirstOrDefault();
        }
        public ObservableCollection<int> DisplayedPagesNumbers
        {
            get => displayedPagesNumbers;
            set
            {
                displayedPagesNumbers = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<int> PagesNumbers { get => pagesNumbers; private set { pagesNumbers = value; OnPropertyChanged(); } }
        public bool IsSelectionEnabled { get => isSelectionEnabled; set { isSelectionEnabled = value; OnPropertyChanged(); } }
        public int SelectedPageNumber
        {
            get { return selectedPageNumber; }
            set { selectedPageNumber = value; OnPropertyChanged(); IsSelectionEnabled = false; OnPropertyChanged(nameof(IsSelectionEnabled)); WaitSelection(); }
        }
        private void LoadPages(int maxPage)
        {
            PagesNumbers = new ObservableCollection<int>();
            var max = maxPage <= 0 ? 1 : maxPage;
            for (int i = 0; i < max; i++)
            {
                PagesNumbers.Add(i + 1);
            }
        }
        public void RefreshPages(int maxPage)
        {
            LoadPages(maxPage);
            RefrashPaginator();
            if (SelectedPageNumber > PagesNumbers.Count)
            {
                SelectedPageNumber = DisplayedPagesNumbers.LastOrDefault();
            }
        }
        public void ChangePage(int page)
        {
            if (page > 0)
            {
                selectedPageNumber = page;
                OnPropertyChanged(nameof(SelectedPageNumber));
            }
        }
        private async void WaitSelection()
        {
            await Task.Delay(300).ContinueWith(_ =>
            {
                IsSelectionEnabled = true;
                OnPropertyChanged(nameof(IsSelectionEnabled));
             
            });
        }
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
