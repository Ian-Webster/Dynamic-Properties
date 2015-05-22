using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models.Json
{
    public class PetViewModel
    {

        public Guid PetID { get; set; }
        public string PetName { get; set; }
        public Guid PetTypeID { get; set; }
        public IEnumerable<PetPropertyModel> Properties { get; set; }   

    }
}