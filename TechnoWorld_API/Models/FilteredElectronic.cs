using System.Collections.Generic;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_API.Models
{
    public class FilteredElectronic
    {
        public FilteredElectronic() { }
        public FilteredElectronic(IEnumerable<Electronic> electronics, int totalFilteredCount)
        {
            Electronics = electronics;
            TotalFilteredCount = totalFilteredCount;
        }

        public IEnumerable<Electronic> Electronics { get; set; }
        public int TotalFilteredCount { get; set; }
    }
}
