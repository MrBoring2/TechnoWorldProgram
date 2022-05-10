using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;
using TechoWorld_DataModels_v2.Entities;

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
    public class FilteredEmployees : FilteredObjects<Employee>
    {
        public FilteredEmployees()
        {

        }
        public FilteredEmployees(IEnumerable<Employee> objects, int totalFiltered) : base(objects, totalFiltered)
        {
        }
    }
}
