using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CubaRest.Model
{
    /// <summary>
    /// Определяет связь класса сущности или перечисления с типом/перечислением Кубы через указание название типа/перечисления Кубы
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
    public class CubaNameAttribute : Attribute
    {
        public CubaNameAttribute(string name) { Name = name; }

        public string Name { get; protected set; }
    }

    /// <summary>
    /// Атрибут EntityProperties анонсирует, что помеченный им интерфейс содержит некоторые из полей сущности Кубы.
    /// В процессе кодогенерации классы сущностей пытаемся отнести к максимально возможному количеству интерфейсов, помеченных
    /// EntityProperties, при вхождении списка свойств интерфейса в список свойств класса
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class EntityPropertiesAttribute : Attribute { }

    #region Атрибуты, накладывающие ограничения на свойства классов сущностей.
    /// <summary>
    /// Базовый класс для атрибутов, накладывающих ограничения на свойства классов сущностей.
    /// Привязка атрибутов для кодогенерации и тестов проверяется именно среди детей этого класса.
    /// </summary>
    public abstract class CubaPropertyRestrictionBase : Attribute
    {
        /// <summary>
        /// Список типов, накладывающих ограничения на свойства классов сущностей
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> ListPropertyRestrictionAttributes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CubaPropertyRestrictionBase)));
        }
    }

    /// <summary>
    /// Обязательное поле сущности
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MandatoryAttribute : CubaPropertyRestrictionBase { }

    /// <summary>
    /// Поле сущности с пометкой "только для чтения"
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : CubaPropertyRestrictionBase { }

    /// <summary>
    /// Транзиентное поле сущности
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TransientAttribute : CubaPropertyRestrictionBase { }
    #endregion


    #region Атрибут Description
    public static class DescriptionAttributeExtension
    {
        /// <summary>
        /// Вспомогательный метод, получающий для значения перечисления значение Description из привязанного атрибута DescriptionAttribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
                return null;

            var attribute = fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description;
        }

        // TODO: Сделать метод, который для произвольного типа будет вытаскивать описание из его атрибута Description
    }
    #endregion
}
