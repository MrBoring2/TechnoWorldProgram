using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnoWorld_Cash.ViewModels.Windows
{
    public class ModalWindowVMBase : BaseWindowVM
    {
        private bool? dialogResult;
        public bool? DialogResult { get { return dialogResult; } set { dialogResult = value; OnPropertyChanged(); } }
    }
}
