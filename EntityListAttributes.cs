using System;
using System.Collections.Generic;

namespace CubaRest
{
    /// <summary>
    /// Дополнительные условия, накладываемые на запрос списка сущностей
    /// </summary>
    public class EntityListAttributes
    {
        /// <summary>Представление, в котором сервер выведет результат</summary>
        public string View { get; set; }

        int? limit;
        /// <summary>Ограничение количества строк в ответе</summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если limit или offset отрицательны</exception>
        public int? Limit
        {
            get => limit;
            set
            {
                if (value == null || value < 0)
                    throw new ArgumentOutOfRangeException("Limit can not be less than zero");

                limit = value;
            }
        }

        int? offset;
        /// <summary>Смещение ответа</summary>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если limit или offset отрицательны</exception>
        public int? Offset
        {
            get => offset;
            set
            {
                if (value == null || value < 0)
                    throw new ArgumentOutOfRangeException("Offset can not be less than zero");

                offset = value;
            }
        }

        /// <summary>Поле для сортировки результатов</summary>
        public string Sort { get; set; }


        /// <summary>
        /// Молчаливое преобразование в Dictionary, чтобы было удобно заполнять список параметров запроса к API
        /// </summary>
        /// <param name="attributes"></param>
        public static implicit operator Dictionary<string, string>(EntityListAttributes attributes)
        {
            if (attributes == null)
                return null;

            Dictionary<string, string> dictionary = new Dictionary<string, string>(4);

            if (!String.IsNullOrEmpty(attributes.View))
                dictionary.Add("view", attributes.View);

            if (attributes.Limit != null)
                dictionary.Add("limit", attributes.Limit.Value.ToString());

            if (attributes.Offset != null)
                dictionary.Add("offset", attributes.Offset.Value.ToString());

            if (!String.IsNullOrEmpty(attributes.Sort))
                dictionary.Add("sort", attributes.Sort);

            return dictionary.Count > 0 ? dictionary : null;
        }
    }
}
