using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models
{
    public class HomeVM
    {

        public IEnumerable<PetVM> Pets { get; set; }
        public IEnumerable<PetTypeVM> PetTypes { get; set; }

    }
}