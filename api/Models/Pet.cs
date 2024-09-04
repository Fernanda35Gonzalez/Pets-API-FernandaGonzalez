using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Pet
    {
        public int Id {get; set;}
        public string Name {get; set;}
        public string Animal {get; set;}
        public int? UserId {get; set;}
        public User? User {get; set;}
    }
}