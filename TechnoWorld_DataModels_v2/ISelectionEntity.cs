using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechoWorld_DataModels_v2
{
    internal interface ISelectionEntity
    {
        event EventHandler OnSelectionChanged;
    }
}
