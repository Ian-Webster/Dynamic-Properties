using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models
{
    public class PetPropertyVM
    {
        public Guid ID { get; set; }
        public PropertyVM Property { get; set; }
        public string Value { get; set; }
    }
}