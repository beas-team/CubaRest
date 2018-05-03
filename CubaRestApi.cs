using CubaRest.Model;
using CubaRest.Model.Reflection;
using CubaRest.Utils;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CubaRest
{
    /// <summary>
    /// Обёртка над API Кубы
    /// </summary>
    public class CubaRestApi
    {
        // TODO: Добавить в CubaRestApi асинхронные запросы

        protected RestClient client;
        protected JsonDeserializer deserializer;

        protected string accessToken = null;
        public string RefreshToken { get; set; } = null;


        /// <summary>
        /// Делегат запроса логина и пароля у пользователя
        /// </summary>
        /// <param name="reason">Причина запроса: неправильный логин/пароль в предыдущую попытку аутентификации, недоступность сервера и т.п.</param>
        /// <param name="usernameCached">Логин, полученный через этот же метод в предыдущую попытку аутентификации</param>
        /// <param name="passwordCached">Пароль, полученный через этот же метод в предыдущую попытку аутентификации</param>
        /// <returns>
        /// (username, password, shouldContinue), где
        /// username и password - введённые пользователем логин и пароль
        /// shouldContinue - признак,  нужно ли пытаться продолжать исполнить запрос или следует его прервать
        /// </returns>
        public delegate (string, string, bool) RequestCredentialsDelegate(RequestCredentialsReason reason = RequestCredentialsReason.Empty,
                                                                    string usernameCached = null,
                                                                    string passwordCached = null);
        public RequestCredentialsDelegate RequestCredentials { get; set; }
        public enum RequestCredentialsReason
        {
            Empty = 0,
            IncorrectCredentials,
        }

        public delegate void RefreshTokenUpdatedDelegate(string newRefreshToken);
        public event RefreshTokenUpdatedDelegate RefreshTokenUpdated;

        #region Конструкторы
        /// <summary>
        /// Создание объекта API на основе endpoint и логина/парола базовой аутентификации
        /// В чистом виде используется только в процедуре получения аутентификации и получении refresh-токена
        /// </summary>
        /// <param name="endpoint">URL для подключения к API</param>
        /// <param name="basicUsername">Имя пользователя предварительной HTTP Basic аутентификации</param>
        /// <param name="basicPassword">Пароль предварительной HTTP Basic аутентификации</param>
        /// <exception cref="CubaInvalidConnectionParametersException"></exception>
        protected CubaRestApi(string endpoint, string basicUsername, string basicPassword)
        {
            try
            {
                client = new RestClient(endpoint) { Authenticator = new HttpBasicAuthenticator(basicUsername, basicPassword) };
                deserializer = new JsonDeserializer();
            }
            catch (ArgumentNullException ex)
            {
                throw new CubaInvalidConnectionParametersException("Empty authentication field is not allowed, all fields are mandatory", ex);
            }
            catch (UriFormatException ex)
            {
                throw new CubaInvalidConnectionParametersException("Incorrect endpoint URI format", ex);
            }
            catch (Exception ex)
            {
                throw new CubaNotImplementedException("Error of unsupported type while creating API connection", ex);
            }
        }

        /// <summary>
        /// Создание объекта API на основе endpoint, логина/парола базовой аутентификации и refresh-токена
        /// </summary>
        /// <param name="endpoint">URL для подключения к API</param>
        /// <param name="basicUsername">Имя пользователя предварительной HTTP Basic аутентификации</param>
        /// <param name="basicPassword">Пароль предварительной HTTP Basic аутентификации</param>
        /// <param name="refreshToken">Refresh токен</param>
        /// <exception cref="CubaInvalidConnectionParametersException"></exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public CubaRestApi(string endpoint, string basicUsername, string basicPassword, string refreshToken = null) 
            : this(endpoint, basicUsername, basicPassword)
        {
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Создание объекта API на основе endpoint, логина/парола базовой аутентификации и основного логина/пароля.
        /// Этот вариант конструктора удобно использовать для тестов. В реальном приложении логин/пароль пользователя на устройстве не храним.
        /// </summary>
        /// <param name="endpoint">URL для подключения к API</param>
        /// <param name="basicUsername">Имя пользователя предварительной HTTP Basic аутентификации</param>
        /// <param name="basicPassword">Пароль предварительной HTTP Basic аутентификации</param>
        /// <param name="username">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <exception cref="CubaInvalidConnectionParametersException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="CubaResponseParsingException">Выбрасывается, если в ответе сервера отсутствует поле refresh_token</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public CubaRestApi(string endpoint, string basicUsername, string basicPassword, string username, string password)
            : this(endpoint, basicUsername, basicPassword)
        {
            RequestRefreshToken(username, password);
        }
        #endregion


        #region Авторизация
        /// <summary>
        /// Получение RefreshToken на основе логина/пароля. Использовать при авторизации пользователя.
        /// Помимо возврата значения refresh-токена заполняет поля access/refresh-токенов.
        /// </summary>
        /// <param name="username">Имя пользователя основной аутентификации</param>
        /// <param name="password">Пароль основной аутентификации</param>
        /// <returns>RefreshToken</returns>
        /// <exception cref="CubaInvalidConnectionParametersException"></exception>
        /// <exception cref="CubaResponseParsingException">Выбрасывается, если в ответе сервера отсутствует поле refresh_token</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public string RequestRefreshToken(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || password == null)
                throw new CubaInvalidConnectionParametersException("Username and/or password are missing");

            var request = new RestRequest("oauth/token", Method.POST);
            request.AddParameter("grant_type", "password");
            request.AddParameter("username", username);
            request.AddParameter("password", password);

            var token = Execute<Dictionary<string, string>>(request);
            RefreshToken = token.TryGetValue("refresh_token") ?? throw new CubaResponseParsingException("Refresh token is missing in server response");
            accessToken = token.TryGetValue("access_token");

            RefreshTokenUpdated?.Invoke(RefreshToken);

            return RefreshToken;
        }
        #endregion
        

        #region Низкоуровневые запросы к API
        /// <summary>
        /// Производит запрос к API и сразу преобразует результат в объект нужного типа.
        /// Проверка соответствия возвращённых данных нужному типу не производится. 
        /// При несоответствии схемы поля заполняются значениями по-умолчанию.
        /// Если запрос вернул ошибку (response.IsSuccessful -> false), выбрасывается исключение.
        /// Это версия метода с автоматическим преобразованием результата в объект.
        /// </summary>
        /// <exception cref="CubaConnectionException"></exception>
        /// <exception cref="CubaAccessException"></exception>
        /// <exception cref="CubaDeserializationException"></exception>
        /// <exception cref="CubaMetaclassNotFoundException"></exception>
        /// <exception cref="CubaViewNotFoundException"></exception>
        /// <exception cref="CubaNotImplementedException"></exception>
        protected T Execute<T>(RestRequest request)
        {
            IRestResponse response = null;

            try
            {
                response = client.Execute(request);

                if (response.IsSuccessful)
                {                   
                    return deserializer.Deserialize<T>(response);
                }
                else
                {
                    if (response.Server == null)
                        throw new CubaConnectionException(response.ErrorMessage);

                    if (response.StatusCode == HttpStatusCode.NotFound) // Либо страница, либо сущность не найдена
                    {
                        // Всё же пытаемся разобрать ответ как JSON. А вдруг.
                        try
                        {
                            var notFoundError = deserializer.Deserialize<Dictionary<string, string>>(response);
                            var details = notFoundError.TryGetValue("details", defaultValue: response.Content);

                            switch (notFoundError.TryGetValue("error"))
                            {
                                case "MetaClass not found":
                                    // Самый частый случай - запросили неправильное название типа Entity
                                    throw new CubaMetaclassNotFoundException(message: details,
                                                    inner: response.ErrorException,
                                                    code: response.StatusCode);

                                case "Entity not found":
                                    // Экземпляр сущности с указанным id не найден
                                    throw new CubaEntityNotFoundException(message: details,
                                                    inner: response.ErrorException,
                                                    code: response.StatusCode);

                                default:
                                    // Ответ пришёл в JSON, но что с ним делать мы не знаем
                                    throw new CubaNotImplementedException(message: details,
                                                    inner: response.ErrorException,
                                                    code: response.StatusCode);
                            }                            
                        }
                        catch (Exception ex) when (!(ex is CubaException)) // Десериализация не удалась, значит пришёл обычный HTML
                        {
                            throw new CubaNotFoundException(response.ErrorMessage ?? response.Content);
                        }
                    }
                        

                    var error = deserializer.Deserialize<Dictionary<string, string>>(response);

                    switch(error.TryGetValue("error"))
                    {
                        case "invalid_grant":   /// Ошибка HTTP Basic аутентификации в процессе запроса токена доступа
                            if (request.Parameters.Any(x => x.Name == "grant_type" && x.Value.ToString() == "refresh_token"))
                                throw new CubaRefreshTokenExpiredException(message: error.TryGetValue("error_description", defaultValue: response.Content),
                                                inner: response.ErrorException,
                                                code: response.StatusCode);
                            else
                                throw new CubaAccessException(message: error.TryGetValue("error_description", defaultValue: response.Content),
                                                inner: response.ErrorException,
                                                code: response.StatusCode);

                        case "unauthorized":    /// В заголовках запроса отсутствует корректный токен доступа в виде поля "Authentication" => "Bearer (тут токен)"
                            throw new CubaAccessException(message: error.TryGetValue("error_description", defaultValue: response.Content),
                                                inner: response.ErrorException,
                                                code: response.StatusCode);

                        case "invalid_token":   /// В запросе передан невалидный токен авторизации или действие токена закончилось
                            throw new CubaAccessTokenExpiredException(message: error.TryGetValue("error_description", defaultValue: response.Content),
                                                inner: response.ErrorException,
                                                code: response.StatusCode);

                        case "View not found":  /// Запросили отсутствующий View
                            throw new CubaViewNotFoundException(message: error.TryGetValue("details", defaultValue: response.Content),
                                            inner: response.ErrorException,
                                            code: response.StatusCode);


                        // TODO: Понять, какие ещё возможны варианты возврата ошибок. 

                        case "Server error":
                        case "server_error":    /// 1. При запросе токена доступа передано пустое поле пароля
                                                /// 2. Could not access HTTP invoker remote service at [cuba_AuthenticationService]; nested exception is java.io.IOException: Did not receive successful HTTP response: status code = 404, status message = [null]

                        case "invalid_request": /// При запросе токена доступа не передано поле "grant_type"

                        default:                /// Ошибка неизвестного типа
                            throw new CubaNotImplementedException(message: response.Content,
                                                inner: response.ErrorException,
                                                code: response.StatusCode);
                    }
                }
            }
            catch (SerializationException ex) // Ошибка разбора JSON. Возникает, когда сервер ответил не в JSON. Вылезает в Deserialize()
            {
                throw new CubaDeserializationException(message: "Response can not be parsed as JSON",
                                        inner: ex,
                                        code: response?.StatusCode ?? HttpStatusCode.OK);
            }
            catch (FormatException ex) // Ошибка приведения успешно разобранного JSON к запрошенной структуре данных. 
            {
                throw new CubaDeserializationException(message: "Error matching data structure to valid JSON response",
                                        inner: ex,
                                        code: response?.StatusCode ?? HttpStatusCode.OK);
            }
            catch (InvalidCastException ex) // Ошибка преобразования типа из JSON ответа сервера в объект
            {
                throw new CubaDeserializationException(message: "Response can not be cast to target object type",
                                        inner: ex,
                                        code: response?.StatusCode ?? HttpStatusCode.OK);
            }
            catch (Exception ex) when (!(ex is CubaException)) // when нужен, т.к. внутри уже выбрасываем CubaNotImplementedException
            {
                throw new CubaNotImplementedException(message: "Unsupported response error",
                                        inner: ex,
                                        code: response?.StatusCode ?? HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// Запрос к API с автоматическим получением/обновлением accessToken на основе RefreshToken
        /// </summary>
        /// <typeparam name="T">Тип данных, в который нужно сериализовать результат</typeparam>
        /// <param name="resource">Ресурс, к которому обращаемся, например "entities/std$Produce" для получения списка сущностей типа std$Produce</param>
        /// <param name="method">GET/POST/и т.д....</param>
        /// <param name="parameters">Параметры запроса</param>
        /// <returns></returns>
        /// <exception cref="CubaInvalidConnectionParametersException">RefreshToken пуст</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        protected T ProceedAuthorizedRequest<T>(string resource, Method method = Method.GET, Dictionary<string, string> parameters = null, object bodyObject = null)
        {
            Exception innerException = null;
            var reason = RequestCredentialsReason.Empty;
            string usernameCached = null;
            string passwordCached = null;

            int attempts;
            for (attempts = 0; attempts < 5; attempts++)
            {
                if (RefreshToken == null)
                {
                    if (RequestCredentials == null)
                        throw new NotImplementedException("Requesting connection credentials is not implemented in client code");

                    (var username, var password, var shouldContinue) = RequestCredentials(reason, usernameCached, passwordCached);
                    usernameCached = username;
                    passwordCached = password;

                    if (shouldContinue)
                    {
                        try
                        {
                            RequestRefreshToken(username, password);
                        }
                        catch (CubaAccessException ex)
                        {
                            innerException = ex;
                            reason = RequestCredentialsReason.IncorrectCredentials;
                            continue;
                        }
                    }
                    else
                        throw new NotImplementedException("Cancelling Cuba request is not implemented yet");
                }

                try
                {
                    if (string.IsNullOrEmpty(accessToken))
                        accessToken = RequestAccessToken(RefreshToken);


                    var request = new RestRequest(resource, method);
                    request.AddHeader("Authorization", $"Bearer {accessToken}");

                    if (parameters != null)
                        foreach (var pair in parameters)
                            request.AddParameter(pair.Key, pair.Value);

                    if (bodyObject != null)
                    {
                        //request.RequestFormat = DataFormat.Json;
                        //request.AddBody(SimpleJson.SimpleJson.SerializeObject(bodyObject));
                        request.AddJsonBody(bodyObject);
                    }

                    return Execute<T>(request);
                }
                catch (CubaAccessTokenExpiredException ex) // В процессе запроса была выдана ошибка экспирации access токена, так что после обновления токена запрос будет повторён
                {
                    innerException = ex;
                    accessToken = null;
                }
                catch (CubaRefreshTokenExpiredException ex) // В процессе запроса была выдана ошибка экспирации refresh токена, так что после обновления токена запрос будет повторён
                {
                    innerException = ex;
                    RefreshToken = null;
                    accessToken = null;
                }
                catch (CubaConnectionException ex)
                {
                    innerException = ex;
                    // В случае проблем со связью также делаем несколько попыток выполнения запроса
                }
            }

            throw new CubaAccessException($"Failed to get correct access token after {attempts} attempt(s)", innerException, HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Выполняет запрос на получение accessToken на основе refreshToken
        /// </summary>
        /// <returns>Свежее значение accessToken</returns>
        /// <exception cref="CubaInvalidConnectionParametersException"></exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        protected string RequestAccessToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new CubaInvalidConnectionParametersException("RefreshToken can not be empty");

            // Получение accessToken из refreshToken
            var request = new RestRequest("oauth/token", Method.POST);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);

            var token = Execute<Dictionary<string, string>>(request);
            return token.TryGetValue("access_token") ?? throw new CubaResponseParsingException("Access token is missing in server response");
        }
        #endregion


        #region Получение списка сущностей

        // (спорно)
        //public List<T> ListEntities<T>(params KeyValuePair<string, string>[] args) where T : Entity
        //{
        //    var type = GetCubaNameForType<T>();
        //    var dic = new Dictionary<string, string>(args.ToDictionary(x => x.Key, x => x.Value));
        //    return ProceedAuthorizedRequest<List<T>>($"entities/{type}", parameters: dic);            
        //}
           


        /// <summary>
        /// Получение списка сущностей с преобразованием сразу в List нужного типа
        /// </summary>
        /// <typeparam name="T">Тип запрашиваемой сущности</typeparam>
        /// <param name="listAttributes">Параметры выборки списка сущностей</param>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если T не может быть преобразован к названию типа данных REST API</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public List<T> ListEntities<T>(EntityListAttributes listAttributes = null) where T : Entity
            => ProceedListEntitiesRequest<List<T>>(GetCubaNameForType<T>(), listAttributes);

        public async Task<List<T>> ListEntitiesAsync<T>(EntityListAttributes listAttributes = null) where T : Entity
            => await Task.Run(() => ProceedListEntitiesRequest<List<T>>(GetCubaNameForType<T>(), listAttributes));

        /// <summary>
        /// Получение списка сущностей с выводом в List<Dictionary<string, string>>
        /// </summary>
        /// <param name="type">Тип сущности REST API, в виде подобном "std$Produce"</param>
        /// <param name="listAttributes">Параметры выборки списка сущностей</param>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если type не соответствует формату названия типа данных REST API</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        // (спорно) [Obsolete("Лучше использовать ListEntities<T> с явным приведением к целевому типу")]
        [Obsolete]
        internal List<Dictionary<string, object>> ListEntities(string type, EntityListAttributes listAttributes = null)
            => ProceedListEntitiesRequest<List<Dictionary<string, object>>>(type, listAttributes);

        /// <summary>
        /// Вспомогательный метод непосредственно для отправки запроса на получение списка сущностей
        /// </summary>
        /// <param name="type">Тип запрашиваемой сущности в формате "std$Produce"</param>
        /// <param name="listAttributes">Параметры выборки списка сущностей</param>
        /// <typeparam name="T">Тип, в который предполагается десериализовать результат, должен быть коллекцией, например List<Dictionary<string, string>></typeparam>
        /// <exception cref="ArgumentOutOfRangeException">Выбрасывается, если limit или offset отрицательные</exception>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если type не соответствует формату названия типа данных REST API</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        protected T ProceedListEntitiesRequest<T>(string type, EntityListAttributes listAttributes = null)
        {
            ValidateMetaclassNameFormat(type);
            return ProceedAuthorizedRequest<T>($"entities/{type}", parameters: listAttributes);
        }
        #endregion


        #region Queries
        public async Task<List<T>> QueryAsync<T>(string queryName, Action<List<T>> callback = null)
        {
            var type = GetCubaNameForType<T>();
            var result = await Task.Run(() => ProceedAuthorizedRequest<List<T>>($"queries/{type}/{queryName}"));
            callback?.Invoke(result);
            return result;
        }
        #endregion

        #region Services
        public T ExecuteService<T>(string service, string method, object bodyObject = null)
            => ProceedAuthorizedRequest<T>($"services/{service}/{method}", Method.POST, null, bodyObject);

        public async Task<T> ExecuteServiceAsync<T>(string service, string method, object bodyObject = null, Action<T> callback = null)
        {
            var result = await Task.Run(() => ProceedAuthorizedRequest<T>($"services/{service}/{method}", Method.POST, null, bodyObject));
            callback?.Invoke(result);
            return result;
        }
        #endregion


        #region Получение экземляра сущности
        /// <summary>
        /// Получение экземляра сущности с преобразованием в целевой тип
        /// </summary>
        /// <typeparam name="T">Тип данных сущности</typeparam>
        /// <param name="id">Id сущности</param>
        /// <param name="view">Представление ответа</param>
        /// <returns>Экземпляр сущности</returns>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если T не может быть преобразован к названию типа данных REST API или id не соответствует формату UUID</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public T GetEntity<T>(string id, string view = null) where T : Entity
            => ProceedGetEntityRequest<T>(GetCubaNameForType<T>(), id, view);

        /// <summary>
        /// Получение экземляра сущности с преобразованием в целевой тип, асинхронная версия
        /// </summary>
        /// <typeparam name="T">Тип данных сущности</typeparam>
        /// <param name="id">Id сущности</param>
        /// <param name="view">Представление ответа</param>
        /// <returns>Экземпляр сущности</returns>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если T не может быть преобразован к названию типа данных REST API или id не соответствует формату UUID</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public async Task<T> GetEntityAsync<T>(string id, string view = null, Action<T> callback = null) where T : Entity
        {
            var result = await Task.Run(() => ProceedGetEntityRequest<T>(GetCubaNameForType<T>(), id, view));
            callback?.Invoke(result);
            return result;
        }

        /// <summary>
        /// Получение экземляра сущности с преобразованием в Dictionary<string, string>
        /// </summary>
        /// <param name="type">Тип сущности в формате REST API</param>
        /// <param name="id">Id сущности</param>
        /// <param name="view">Представление ответа</param>
        /// <returns>Экземпляр сущности</returns>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если type не соответствует формату названия типа данных REST API или id не соответствует формату UUID</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        public Dictionary<string, object> GetEntity(string type, string id, string view = null) 
            => ProceedGetEntityRequest<Dictionary<string, object>>(type, id, view);

        /// <summary>
        /// Вспомогательный метод непосредственно для отправки запроса на сущности по id
        /// </summary>
        /// <param name="type">Тип запрашиваемой сущности в формате "std$Produce"</param>
        /// <param name="id">Id запрашиваемой сущности в формате UUID</param>
        /// <param name="view">Представление ответа</param>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если type или id не соответствуют формату названия типа и UUID соответственно</exception>
        /// <exception cref="CubaException">Все те же самые исключения, что и у Execute()</exception>
        protected T ProceedGetEntityRequest<T>(string type, string id, string view = null)
        {
            ValidateMetaclassNameFormat(type);
            ValidateUuidFormat(id);

            var parameters = view != null ? new Dictionary<string, string> { { "view", view } } : null;

            return ProceedAuthorizedRequest<T>($"entities/{type}/{id}", method: Method.GET, parameters: parameters );
        }
        #endregion


        #region Рефлексия
        // TEST: Протестировать методы рефлексии Кубы
        // TODO: Написать комментарии к методам рефлексии Кубы

        /// <summary>
        /// Получение информации о всех типах Кубы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">Фильтрует выдачу данных только по типам Кубы, название которых начинается с prefix</param>
        /// <returns></returns>
        public List<EntityType> ListTypes(string prefix = null)
        {
            var result = ProceedAuthorizedRequest<List<EntityType>>($"metadata/entities", method: Method.GET);
            return string.IsNullOrEmpty(prefix) ? result : result.FindAll(x => x.EntityName.StartsWith(prefix));
        }

        /// <summary>
        /// Получение информации о типе Кубы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public EntityType GetTypeMetadata(string type)
        {
            ValidateMetaclassNameFormat(type);
            return ProceedAuthorizedRequest<EntityType>($"metadata/entities/{type}", method: Method.GET);
        }

        /// <summary>
        /// Получение информации о всех перечислениях Кубы
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix">Фильтрует выдачу данных только по типам Кубы, название которых начинается с prefix</param>
        /// <returns></returns>
        public List<EnumType> ListEnums(string prefix = null)
        {
            var result = ProceedAuthorizedRequest<List<EnumType>>($"metadata/enums", method: Method.GET);
            return string.IsNullOrEmpty(prefix) ? result : result.FindAll(x => x.Name.StartsWith(prefix));
        }

        public EnumType GetEnumMetadata(string type)
        {
            return ProceedAuthorizedRequest<EnumType>($"metadata/enums/{type}", method: Method.GET);
        }

        public static void ValidateEntity(Entity entity)
        {
            // TODO: Реализовать проверку консистентности значений полей на основе рефлексии Кубы
            throw new NotImplementedException();
        }

        /// <summary>
        /// Получение списка встроенных типов данных Кубы (string, boolean, double, date и пр.)
        /// </summary>
        /// <returns></returns>
        public List<string> ListDatatypes()
        {
            var response = ProceedAuthorizedRequest<List<Dictionary<string, string>>>($"metadata/datatypes", method: Method.GET);
            return new List<string>(response.Select(x => x.TryGetValue("id")));
        }

        public List<EntityView> ListEntityViews(string cubaType)
        {
            ValidateMetaclassNameFormat(cubaType);
            var result = ProceedAuthorizedRequest<List<EntityView>>($"metadata/entities/{cubaType}/views", method: Method.GET);
            return result;
        }
        #endregion


        #region Вспомогательные методы
        /// <summary>
        /// Преобразовывает названия классов сущностей вроде StdProduce в названия типов REST API вроде std$Produce
        /// </summary>
        /// <exception cref="CubaEntityMappingSchemeException">Выбрасывается, если тип не может быть преобразован в название сущности REST API</exception>
        public static string GetCubaNameForType(Type type)
        {
            foreach (var attribute in type.GetCustomAttributes(typeof(CubaNameAttribute), false))
            { 
                string name = (attribute as CubaNameAttribute)?.Name;
                return !string.IsNullOrEmpty(name) ? name : throw new CubaEntityMappingSchemeException("Invalid REST API metaclass name annotation");
            }

            throw new CubaEntityMappingSchemeException($"{type.GetNameWithDeclaring()} class has no mandatory CubaMetaclass attribute");

            //// Старый способ, на основе преобразования имени
            //string name = type.Name;

            //string declaringName = type.DeclaringType?.Name;
            //if (declaringName != null)
            //    name = declaringName + name;

            //try
            //{
            //    char secondCapital = name.Substring(1).First(c => Char.IsUpper(c));
            //    int secondCapitalIndex = name.IndexOf(secondCapital);
            //    return $"{name.Substring(0, secondCapitalIndex).ToLower()}${name.Substring(secondCapitalIndex)}";
            //}
            //catch (Exception ex)
            //{
            //    throw new CubaInvalidFormatException($"\"{name}\" can not be converted to REST API name", ex);
            //}
        }

        /// <summary>
        /// Преобразовывает названия классов сущностей вроде StdProduce в названия типов REST API вроде std$Produce. Generic-версия метода.
        /// </summary>
        /// <exception cref="CubaEntityMappingSchemeException">Выбрасывается, если тип не может быть преобразован в название сущности REST API</exception>
        public static string GetCubaNameForType<T>() => GetCubaNameForType(typeof(T));

        /// <summary>
        /// Проверяет регулярным выражением соответствие строки формату названия типа данных REST API: "xxx$XxxXxxxXxxx"
        /// </summary>
        /// <param name="text">Название типа данных</param>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если строка не соответствует формату названия типа данных REST API</exception>
        public static void ValidateMetaclassNameFormat(string text)
        {
            Regex guidRegEx = new Regex(@"^[a-z]{0,10}\$([A-Z]{1}[a-z0-9]{0,100}){1,10}$");

            if (String.IsNullOrEmpty(text) || !guidRegEx.IsMatch(text))
                throw new CubaInvalidFormatException($"\"{text}\" does not match REST API metaclass format: \"xxx$XxxxXxxxXxxx\"");
        }

        /// <summary>
        /// Проверяет регулярным выражением соответствие строки формату названия перечисления REST API: "xxx.xxx.xxx.xxx.Xxxxxx"
        /// </summary>
        /// <param name="text">Название типа данных</param>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если строка не соответствует формату названия типа данных REST API</exception>
        public static void ValidateEnumNameFormat(string text)
        {
            Regex guidRegEx = new Regex(@"^([a-z]{1,100}.{1}){1,10}[A-Z]{1}[a-zA-Z0-9\$]{0,100}$");

            if (String.IsNullOrEmpty(text) || !guidRegEx.IsMatch(text))
                throw new CubaInvalidFormatException($"\"{text}\" does not match REST API enum format: \"xxx.xxx.xxx.xxx.Xxxxxx\"");
        }

        /// <summary>
        /// Проверяет регулярным выражением соответствие строки формату названия типа данных REST API: "xxx$XxxXxxxXxxx"
        /// </summary>
        /// <param name="text">Название типа данных</param>
        /// <exception cref="CubaInvalidFormatException">Выбрасывается, если строка не соответствует формату названия типа данных REST API</exception>
        public static void ValidateUuidFormat(string text)
        {
            if (String.IsNullOrEmpty(text) || !text.IsValidUuid())
                throw new CubaInvalidFormatException($"\"{text}\" is not valid UUID format");
        }
        #endregion
    }    
}