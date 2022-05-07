using System.Collections.Generic;
using TechoWorld_DataModels_v2;

namespace TechoWorld_DataModels_v2.Abstractions
{
    public class FilteredObjects<T> where T : BaseEntity
    {
        public FilteredObjects()
        {

        }

        public FilteredObjects(IEnumerable<T> objects, int totalFiltered)
        {
            Objects = objects;
            TotalFiltered = totalFiltered;
        }

        public IEnumerable<T> Objects { get; set; }

        public int TotalFiltered { get; set; }
    }
}
