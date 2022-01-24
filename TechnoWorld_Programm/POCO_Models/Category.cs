using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoWorld_Programm.POCO_Models;

namespace TechnoWorld_Terminal.POCO_Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public ICollection<Electronic> Electronics { get; set; }
    }
}
