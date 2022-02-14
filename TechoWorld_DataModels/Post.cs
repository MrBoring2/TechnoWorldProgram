﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;



namespace TechoWorld_DataModels
{
    public partial class Post
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