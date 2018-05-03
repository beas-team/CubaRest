using System;
using System.Collections.Generic;
using System.Text;

namespace CubaRest.Model.Reflection
{
    public class EntityView
    {
        public string Name { get; set; }
        public string Entity { get; set; }
        public List<object> Properties { get; set; }
    }
}
