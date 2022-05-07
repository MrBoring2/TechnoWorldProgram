using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Helpers.Models
{
    public class SortParameter : INotifyPropertyChanged
    {
        private bool isDescening;
        public SortParameter(string title, string property)
        {
            Title = title;
            Property = property;
            IsAcsending = true;
        }

        public string Title { get; set; }
        public string Property { get; set; }
        public bool IsAcsending
        {
            get => isDescening;
            set { isDescening = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
