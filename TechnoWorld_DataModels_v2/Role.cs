using Newtonsoft.Json;
using System.Collections.Generic;


namespace TechoWorld_DataModels_v2
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
