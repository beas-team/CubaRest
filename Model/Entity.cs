using CubaRest;
using CubaRest.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubaRest.Model
{
    /// <summary>
    /// Стандартная сущность REST API. Общий предок для всех сущностей.
    /// </summary>
    public abstract class Entity
    {
        public string _entityName { get; set; }
        public string _instanceName { get; set; }
        public string __securityToken { get; set; }

        public override string ToString() => _instanceName;
    }

    /// <summary>
    /// Тип сущности, у которой в качестве Id используется строка UUID
    /// </summary>
    [EntityProperties]
    public interface IUuidEntity
    {
        [Description("ID")]
        string Id { get; set; }
    }

    /// <summary>
    /// Тип сущности, у которой есть Бриф
    /// </summary>
    [EntityProperties]
    public interface IBriefEntity
    {
        string Brief { get; set; }
    }

    [EntityProperties]
    public interface IStandardEntity
    {
        string CreatedBy { get; set; }
        DateTime CreateTs { get; set; }
        string DeletedBy { get; set; }
        DateTime DeleteTs { get; set; }
        string UpdatedBy { get; set; }
        DateTime UpdateTs { get; set; }
        int Version { get; set; }
    }
}