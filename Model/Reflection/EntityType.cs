using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubaRest.Model.Reflection
{
    /// <summary>
    /// Тип данных сущности Кубы
    /// </summary>
    public class EntityType
    {
        /// <summary>
        /// Название типа
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Набор полей сущности
        /// </summary>
        public List<EntityField> Properties { get; set; }
    }
}
