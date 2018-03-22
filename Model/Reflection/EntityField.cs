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
    public class EntityField
    {
        // Пример данных:
        //"name": "brief",
        //"attributeType": "DATATYPE",
        //"type": "string",
        //"cardinality": "NONE",
        //"mandatory": true,
        //"readOnly": false,
        //"description": "Бриф",
        //"persistent": true,
        //"transient": false

        public string Name { get; set; }
        public AttributeType AttributeType { get; set; } // тип поля: значение, связь с другой таблицей или перечисление
        public string Type { get; set; } // один из стандартных типов или название типа сущности
        public Cardinality Cardinality { get; set; }
        public bool Mandatory { get; set; }
        public bool ReadOnly { get; set; }
        public string Description { get; set; }
        public bool Persistent { get; set; }
        public bool Transient { get; set; }

        public override string ToString() => $"{Name}, {Description}, {Type}";
    }

    public enum AttributeType
    {
        DATATYPE,
        COMPOSITION,
        ASSOCIATION,
        ENUM,
    }

    public enum Cardinality
    {
        NONE,
        ONE_TO_ONE,
        ONE_TO_MANY,
        MANY_TO_ONE,
        MANY_TO_MANY,
    }
}
