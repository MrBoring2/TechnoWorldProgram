using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Terminal.Models
{
    public class SortParameter : NotifyPropertyChangedModel
    {
        private bool isDescening;
        public SortParameter(string title, string property)
        {
            Title = title;
            Property = property;
        }

        public string Title { get; set; }
        public string Property { get; set; }
        public bool IsDescening
        {
            get => isDescening; 
            set { isDescening = value; OnPropertyChanged(); }
        }
    }
}
