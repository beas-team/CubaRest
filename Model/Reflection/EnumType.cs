using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubaRest.Model.Reflection
{
    /// <summary>
    /// Перечисление Кубы
    /// </summary>
    public class EnumType
    {
        /// <summary>
        /// Название перечисления
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значения
        /// </summary>
        public List<EnumField> Values { get; set; }
    }
}
