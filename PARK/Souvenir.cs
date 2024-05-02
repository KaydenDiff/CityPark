using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARK
{
    public class Souvenir
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }

        public int category_souvenir_id { get; set; }
        public string CategoryName { get; set; } // Имя категории
        public string photo { get; set; }
    }
}
