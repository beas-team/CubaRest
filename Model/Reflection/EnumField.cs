using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubaRest.Model.Reflection
{
    /// <summary>
    /// Поле сущности Кубы
    /// </summary>
    public class EnumField
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Caption { get; set; }

        public override string ToString() => $"{Name}, {Caption}";
    }    
}
