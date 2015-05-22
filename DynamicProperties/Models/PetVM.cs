using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models
{
    public class PetVM
    {
        public Guid PetID { get; set; }
        [Required(ErrorMessage = "You must enter pet name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "You must select pet type")]
        public Guid PetTypeID { get; set; }
        public List<PetPropertyVM> Properties { get; set; }

    }
}