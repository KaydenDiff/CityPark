using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PARK
{
    public class UsersList
    {

      
        
            public int id { get; set; }
            public string login { get; set; }
            public string surname { get; set; }
            public string name { get; set; }
        public string patronymic { get; set; }
        public string roleName { get; set; }
        public string fullName { get; set; }
        public int Role_id { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        
        

    }
}
