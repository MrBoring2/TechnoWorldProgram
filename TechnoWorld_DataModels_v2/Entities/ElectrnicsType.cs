using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TechoWorld_DataModels_v2.Abstractions;

namespace TechoWorld_DataModels_v2.Entities
{
    public partial class ElectrnicsType : BaseEntity, ISelectionEntity
    {
        private bool isSelected;
        public ElectrnicsType()
        {
        }
        public event EventHandler OnSelectionChanged;

        public int TypeId { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        [NotMapped]
        public bool IsSelected { get=>isSelected; set { isSelected = value; OnSelectionChanged?.Invoke(this, new EventArgs()); } }
        [JsonIgnore]
        public virtual ICollection<Electronic> Electronics { get; set; }
    }
}
