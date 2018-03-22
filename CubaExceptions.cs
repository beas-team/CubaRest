using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CubaRest
{
    /// <summary>
    /// Исключение общего вида для REST API Кубы.
    /// Используется только для фильтрации исключений как общий родитель всех остальных исключений Кубы
    /// </summary>
    public abstract class CubaException : Exception
    {
        public CubaException(string message = null, Exception inner = null) : base(message, inner) { }
    }

    /// <summary>
    /// Выбрасывается при изначально некорректно заданных параметрах подключения к API. Пустой логин/пароль, некорректный endpoint и т.п.
    /// </summary>
    public class CubaInvalidConnectionParametersException : CubaException
    {
        public CubaInvalidConnectionParametersException(string message, Exception inner = null)
                             : base(message, inner) { }
    }

    /// <summary>
    /// Выбрасывается при недоступности сервера или при некорректном ответе сервера
    /// </summary>
    public class CubaConnectionException : CubaException
    {
        public CubaConnectionException(string message, Exception inner = null)
                             : base(message, inner) { }
    }

    /// <summary>
    /// Выбрасывается при обнаружении несоответствия содержимого переменной назначенному формату
    /// </summary>
    public class CubaInvalidFormatException : CubaException
    {
        public CubaInvalidFormatException(string message, Exception inner = null)
                             : base(message, inner) { }
    }

    /// <summary>
    /// Выбрасывается при обнаружении несоответствия локальной схемы сущностей схеме сущностей в REST API
    /// </summary>
    public class CubaEntityMappingSchemeException : CubaException
    {
        public CubaEntityMappingSchemeException(string message, Exception inner = null)
                             : base(message, inner) { }
    }



    /// <summary>
    /// Исключение, связанное с ответом сервера Кубы. Отличительный признак - есть код ответа http и название ошибки.
    /// Успешное получение ответа не говорит об успешности его разбора.
    /// </summary>
    public class CubaResponseException : CubaException
    {
        HttpStatusCode code;

        public CubaResponseException(string message = null,
                             Exception inner = null,
                             HttpStatusCode code = HttpStatusCode.OK)
                             : base(message, inner)
        {
            this.code = code;
        }
    }

    /// <summary>
    /// Выбрасывается, если запись отсутствует на сервере или отсутствует сам сервер
    /// </summary>
    public class CubaNotFoundException : CubaResponseException
    {
        public CubaNotFoundException(string message, Exception inner = null)
                             : base(message, inner, HttpStatusCode.NotFound) { }
    }

    /// <summary>
    /// Исключение общего вида, связанное с разбором успешно полученного ответа сервера
    /// </summary>
    public class CubaResponseParsingException : CubaResponseException
    {
        public CubaResponseParsingException(string message) : base(message) { }
        public CubaResponseParsingException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }

    /// <summary>
    /// Ошибка десериализации ответа сервера из JSON
    /// </summary>
    public class CubaDeserializationException : CubaResponseParsingException
    {
        public CubaDeserializationException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }

    /// <summary>
    /// Ошибка доступа
    /// </summary>
    public class CubaAccessException : CubaResponseException
    {
        public CubaAccessException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }

    /// <summary>
    /// Требуется обновление токена accessToken
    /// </summary>
    public class CubaAccessTokenExpiredException : CubaAccessException
    {
        public CubaAccessTokenExpiredException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }

    /// <summary>
    /// Тип запрошеной сущности отсутствует в структуре данных сервера
    /// </summary>
    public class CubaMetaclassNotFoundException : CubaResponseException
    {
        public CubaMetaclassNotFoundException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }

    /// <summary>
    /// Сущность с указанным id на сервере отсутствует
    /// </summary>
    public class CubaEntityNotFoundException : CubaResponseException
    {
        public CubaEntityNotFoundException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }

    /// <summary>
    /// Запрошенный view отсутствует в структуре данных сервера
    /// </summary>
    public class CubaViewNotFoundException : CubaResponseException
    {
        public CubaViewNotFoundException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }








    /// <summary>
    /// Исключение неизвестного типа
    /// </summary>
    public class CubaNotImplementedException : CubaResponseException
    {
        public CubaNotImplementedException(string message, Exception inner) : base(message, inner) { }
        public CubaNotImplementedException(string message, Exception inner, HttpStatusCode code) : base(message, inner, code) { }
    }
}
