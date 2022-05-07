using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechoWorld_DataModels_v2.Abstractions
{
    public class BaseEntity : INotifyPropertyChanged
    {
        public object GetProperty(string property)
        {
            return GetType().GetProperty(property).GetValue(this);
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
