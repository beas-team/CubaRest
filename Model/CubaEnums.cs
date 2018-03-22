using System.ComponentModel;

namespace CubaRest.Model
{    
    // Встроенные перечисления Кубы com.haulmont.cuba

    [CubaName("com.haulmont.cuba.core.entity.FtsChangeType")]
    public enum FtsChangeType
    {
        /// <summary>FtsChangeType.DELETE</summary>
        [Description("FtsChangeType.DELETE")]
        DELETE,

        /// <summary>FtsChangeType.INSERT</summary>
        [Description("FtsChangeType.INSERT")]
        INSERT,

        /// <summary>FtsChangeType.UPDATE</summary>
        [Description("FtsChangeType.UPDATE")]
        UPDATE,
    }

    [CubaName("com.haulmont.cuba.core.entity.ScheduledTaskDefinedBy")]
    public enum ScheduledTaskDefinedBy
    {
        /// <summary>Bean</summary>
        [Description("Bean")]
        BEAN,

        /// <summary>Class</summary>
        [Description("Class")]
        CLASS,

        /// <summary>Script</summary>
        [Description("Script")]
        SCRIPT,
    }

    [CubaName("com.haulmont.cuba.core.entity.SchedulingType")]
    public enum SchedulingType
    {
        /// <summary>Cron</summary>
        [Description("Cron")]
        CRON,

        /// <summary>Fixed Delay</summary>
        [Description("Fixed Delay")]
        FIXED_DELAY,

        /// <summary>Period</summary>
        [Description("Period")]
        PERIOD,
    }

    [CubaName("com.haulmont.cuba.core.global.SendingStatus")]
    public enum SendingStatus
    {
        /// <summary>Отправка не удалась</summary>
        [Description("Отправка не удалась")]
        NOTSENT = 300,

        /// <summary>Поставлено на отправку</summary>
        [Description("Поставлено на отправку")]
        QUEUE = 0,

        /// <summary>Отправляется</summary>
        [Description("Отправляется")]
        SENDING = 100,

        /// <summary>Отправлено</summary>
        [Description("Отправлено")]
        SENT = 200,
    }

    [CubaName("com.haulmont.cuba.gui.app.security.entity.AttributePermissionVariant")]
    public enum AttributePermissionVariant
    {
        /// <summary>AttributePermissionVariant.HIDE</summary>
        [Description("AttributePermissionVariant.HIDE")]
        HIDE = 30,

        /// <summary>AttributePermissionVariant.MODIFY</summary>
        [Description("AttributePermissionVariant.MODIFY")]
        MODIFY = 10,

        /// <summary>AttributePermissionVariant.NOTSET</summary>
        [Description("AttributePermissionVariant.NOTSET")]
        NOTSET = 40,

        /// <summary>AttributePermissionVariant.READ_ONLY</summary>
        [Description("AttributePermissionVariant.READ_ONLY")]
        READ_ONLY = 20,
    }

    [CubaName("com.haulmont.cuba.gui.app.security.entity.PermissionVariant")]
    public enum PermissionVariant
    {
        /// <summary>Разрешено</summary>
        [Description("Разрешено")]
        ALLOWED = 10,

        /// <summary>Запрещено</summary>
        [Description("Запрещено")]
        DISALLOWED = 20,


        NOTSET = 30,
    }

    [CubaName("com.haulmont.cuba.gui.app.security.entity.UiPermissionVariant")]
    public enum UiPermissionVariant
    {
        /// <summary>скрыть</summary>
        [Description("скрыть")]
        HIDE = 20,


        NOTSET = 30,

        /// <summary>только для чтения</summary>
        [Description("только для чтения")]
        READ_ONLY = 10,

        /// <summary>изменение</summary>
        [Description("изменение")]
        SHOW = 40,
    }

    [CubaName("com.haulmont.cuba.security.entity.ConstraintCheckType")]
    public enum ConstraintCheckType
    {
        /// <summary>Проверка в базе данных</summary>
        [Description("Проверка в базе данных")]
        DATABASE,

        /// <summary>Проверка в базе данных и памяти</summary>
        [Description("Проверка в базе данных и памяти")]
        DATABASE_AND_MEMORY,

        /// <summary>Проверка в памяти</summary>
        [Description("Проверка в памяти")]
        MEMORY,
    }

    [CubaName("com.haulmont.cuba.security.entity.ConstraintOperationType")]
    public enum ConstraintOperationType
    {
        /// <summary>Все</summary>
        [Description("Все")]
        ALL,

        /// <summary>Создание</summary>
        [Description("Создание")]
        CREATE,

        /// <summary>Специальная операция</summary>
        [Description("Специальная операция")]
        CUSTOM,

        /// <summary>Удаление</summary>
        [Description("Удаление")]
        DELETE,

        /// <summary>Чтение</summary>
        [Description("Чтение")]
        READ,

        /// <summary>Модификация</summary>
        [Description("Модификация")]
        UPDATE,
    }

    [CubaName("com.haulmont.cuba.security.entity.EntityLogItem$Type")]
    public enum EntityLogItemType
    {
        /// <summary>Создание</summary>
        [Description("Создание")]
        CREATE,

        /// <summary>Удаление</summary>
        [Description("Удаление")]
        DELETE,

        /// <summary>Изменение</summary>
        [Description("Изменение")]
        MODIFY,

        /// <summary>Восстановление</summary>
        [Description("Восстановление")]
        RESTORE,
    }

    [CubaName("com.haulmont.cuba.security.entity.PermissionType")]
    public enum PermissionType
    {
        /// <summary>Атрибут сущности</summary>
        [Description("Атрибут сущности")]
        ENTITY_ATTR = 30,

        /// <summary>Операция с сущностью</summary>
        [Description("Операция с сущностью")]
        ENTITY_OP = 20,

        /// <summary>Экран</summary>
        [Description("Экран")]
        SCREEN = 10,

        /// <summary>Специфическое</summary>
        [Description("Специфическое")]
        SPECIFIC = 40,

        /// <summary>Интерфейс</summary>
        [Description("Интерфейс")]
        UI = 50,
    }

    [CubaName("com.haulmont.cuba.security.entity.RoleType")]
    public enum RoleType
    {
        /// <summary>Запрещающая</summary>
        [Description("Запрещающая")]
        DENYING = 30,

        /// <summary>Только чтение</summary>
        [Description("Только чтение")]
        READONLY = 20,

        /// <summary>Стандартная</summary>
        [Description("Стандартная")]
        STANDARD = 0,

        /// <summary>Супер</summary>
        [Description("Супер")]
        SUPER = 10,
    }

    [CubaName("com.haulmont.cuba.security.entity.SessionAction")]
    public enum SessionAction
    {
        /// <summary>Истечение срока</summary>
        [Description("Истечение срока")]
        EXPIRATION = 3,

        /// <summary>Логин</summary>
        [Description("Логин")]
        LOGIN = 1,

        /// <summary>Выход</summary>
        [Description("Выход")]
        LOGOUT = 2,

        /// <summary>Подмена пользователя</summary>
        [Description("Подмена пользователя")]
        SUBSTITUTION = 5,

        /// <summary>Прервана</summary>
        [Description("Прервана")]
        TERMINATION = 4,
    }
    /// TEST: Попытка приведения Enum к несуществующему значению
}
