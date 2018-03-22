using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CubaRest.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Переопределяет стандартный Dictionary.TryGetValue(). Если ключ отсутствует в словаре, возвращается значение defaultValue
        /// </summary>
        /// <param name="key">Ключ словаря</param>
        /// <param name="defaultValue">Значение по-умолчанию для возврата, если переданный ключ в словаре отсутствует</param>
        /// <returns>Возвращает значение словаря по ключу или значение defaultValue, если ключ отсутствует</returns>
        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value = dictionary.TryGetValue(key, out value) ? value : defaultValue;
            return value;
        }

        /// <summary>
        /// Расширяет string: добавляет метод проверки текущей строки на соответствие формату UUID
        /// </summary>
        /// <returns>true, если строка соответствует формату</returns>
        public static bool IsValidUuid(this string text)
        {
            if (String.IsNullOrEmpty(text))
                return false;

            Regex guidRegEx = new Regex(@"^(([0-9a-f]){8}-([0-9a-f]){4}-([0-9a-f]){4}-([0-9a-f]){4}-([0-9a-f]){12})$");
            return guidRegEx.IsMatch(text);
        }

        /// <summary>
        /// Расширяет string: приводит строку к стилю PascalCase
        /// </summary>
        public static string ToPascalCase(this string the_string)
        {
            // If there are 0 or 1 characters, just return the string.
            if (the_string == null) return the_string;
            if (the_string.Length < 2) return the_string.ToUpper();

            // Split the string into words.
            string[] words = the_string.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }
        //public static string CapitalizeFirst(this string text) 
        //    => string.IsNullOrEmpty(text) ? null : text.First().ToString().ToUpper() + text.Substring(1);
        
        /// <summary>
        /// Расширяет Type. Имя через точку вместе с именем DeclaringType. 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetNameWithDeclaring(this Type type) 
            => type.DeclaringType != null ? $"{type?.DeclaringType.Name}.{type.Name}" : type.Name;
    }
}
