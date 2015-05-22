using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models
{
    public class PetTypeVM
    {

        public Guid PetTypeID { get; set; }
        public string Name { get; set; }
        public List<PropertyVM> Properties { get; set; }

    }
}