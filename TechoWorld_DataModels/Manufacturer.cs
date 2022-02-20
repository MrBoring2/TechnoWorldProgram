using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechoWorld_DataModels
{
    public partial class Manufacturer : BaseEntity, ISelectionEntity
    {
        private bool isSelected;
        public Manufacturer()
        {
            Electronics = new HashSet<Electronic>();
        }

        public int ManufacturerId { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public virtual ICollection<Electronic> Electronics { get; set; }
        [NotMapped]
        public bool IsSelected { get => isSelected; set { isSelected = value; OnSelectionChanged?.Invoke(this, new EventArgs()); } }

        public event EventHandler OnSelectionChanged;
    }
}
