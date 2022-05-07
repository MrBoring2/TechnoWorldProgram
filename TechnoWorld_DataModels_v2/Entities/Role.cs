using Newtonsoft.Json;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class Role : BaseEntity
    {
        public Role()
        {
            Employees = new HashSet<Employee>();
        }

        public int RoleId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
