using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class Post : BaseEntity
    {
        public Post()
        {
            Employees = new HashSet<Employee>();
        }

        public int PostId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
