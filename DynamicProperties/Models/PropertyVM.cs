using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicProperties.Models
{
    public class PropertyVM
    {
        public int PropertyID { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyName { get; set; }
        public bool Required { get; set; }
    }
}