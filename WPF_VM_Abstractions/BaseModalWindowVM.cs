using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_VM_Abstractions
{
    public class BaseModalWindowVM : BaseWindowVM
    {
        private bool? dialogResult;
        public bool? DialogResult { get { return dialogResult; } set { dialogResult = value; OnPropertyChanged(); } }
    }
}
