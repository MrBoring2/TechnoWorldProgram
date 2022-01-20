using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Programm.Models
{
    public class SortParameter
    {
        public SortParameter(string title, string property)
        {
            Title = title;
            Property = property;
        }

        public string Title { get; set; }
        public string Property { get; set; }
        public bool IsDescening { get; set; }
    }
}
