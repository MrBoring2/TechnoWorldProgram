using System.Collections.Generic;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_API.Models
{
    public class FilteredElectronic
    {
        public FilteredElectronic() { }
        public FilteredElectronic(List<Electronic> electronics, int totalFilteredCount)
        {
            Electronics = electronics;
            TotalFilteredCount = totalFilteredCount;
        }

        public List<Electronic> Electronics { get; set; }
        public int TotalFilteredCount { get; set; }
    }
}
