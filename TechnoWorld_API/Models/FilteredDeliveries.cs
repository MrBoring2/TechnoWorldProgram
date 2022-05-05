using System.Collections.Generic;
using TechoWorld_DataModels_v2;

namespace TechnoWorld_API.Models
{
    public class FilteredDeliveries
    {
        public FilteredDeliveries() { }
        public FilteredDeliveries(IEnumerable<Delivery> deliveries, int totalFilteredCount)
        {
            Deliveries = deliveries;
            TotalFilteredCount = totalFilteredCount;
        }

        public IEnumerable<Delivery> Deliveries { get; set; }
        public int TotalFilteredCount { get; set; }
    }
}
