using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Pet
{
    public class CreatePetsRequestDto
    {
        public string Name { get; set; }
        public string Animal { get; set; }
    }
}