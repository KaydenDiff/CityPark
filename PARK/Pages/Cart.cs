using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARK.Pages
{
    public class Cart
    {
        public int id { get; set; }
        public int quantity { get; set; }
        public decimal total { get; set; }
        public int souvenir_id { get; set; }
        public int user_id { get; set; }
        public string phone { get; set; }
        public decimal TotalPrice => quantity * total;
        public string FullName { get; set; }
        public string SouvenirName { get; set; }

        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
