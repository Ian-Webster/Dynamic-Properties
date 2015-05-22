using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models.Json
{
    public class PetPropertyModel
    {

        public int PropertyID { get; set; }
        public string PropertyLabel { get; set; }
        public string PropertyValue { get; set; }
        public bool PropertyValue_Bool { get; set; }
        public string PropertyName { get; set; }

    }
}