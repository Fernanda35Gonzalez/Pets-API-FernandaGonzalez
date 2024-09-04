using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class User
    {
        public int Id {get; set;}
        public string FirstName {get; set;}
        public string LastName {get; set;}
        public int Age {get; set;}
        public DateTime CreatedAt {get; set;} = DateTime.Now;
        public List<Pet> pets {get; set;} = new List<Pet>();
    }
}