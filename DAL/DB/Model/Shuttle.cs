using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DB.Model
{
    public class Shuttle
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; } 
        public bool IsAvailable { get; set; }
    }
}
