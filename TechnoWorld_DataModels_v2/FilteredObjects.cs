using System.Collections.Generic;

namespace TechoWorld_DataModels_v2
{
    public class FilteredDeliveries : FilteredObjects<Delivery>
    {
        public FilteredDeliveries()
        {
        }

        public FilteredDeliveries(IEnumerable<Delivery> objects, int totalFiltered) : base(objects, totalFiltered)
        {
        }
    }

    public class FilteredElectronic : FilteredObjects<Electronic>
    {
        public FilteredElectronic()
        {
        }

        public FilteredElectronic(IEnumerable<Electronic> objects, int totalFiltered) : base(objects, totalFiltered)
        {
        }
    }

    public class FilteredOrders : FilteredObjects<Order>
    {
        public FilteredOrders()
        {
        }

        public FilteredOrders(IEnumerable<Order> objects, int totalFiltered) : base(objects, totalFiltered)
        {
        }
    }
}
