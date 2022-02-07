using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechoWorld_DataModels
{
    public partial class ElectrnicsType
    {
        public ElectrnicsType()
        {
        }

        public int TypeId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; }
    }
}
