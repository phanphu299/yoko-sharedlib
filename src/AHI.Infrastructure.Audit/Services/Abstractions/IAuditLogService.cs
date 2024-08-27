using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.SystemContext.Enum;

namespace AHI.Infrastructure.Audit.Service.Abstraction
{
    public interface IAuditLogService
    {
        AppLevel AppLevel { get; }
        void SetAppLevel(AppLevel appLevel);
        void SetPropertyName(string applicationName, string subscriptionName = null, string projectName = null);

        Task SendLogAsync(ActivityLogMessage message);

        Task SendLogAsync(Guid id, string entity, Enum action, ActionStatus status, string entityId, string entityName = null, params object[] payload);
        Task SendLogAsync(Guid id, string entity, Enum action, ActionStatus status, object entityId = null, string entityName = null, params object[] payload);
        Task SendLogAsync<T>(Guid id, string entity, Enum action, ActionStatus status, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payload);

        Task SendLogAsync(string entity, Enum action, ActionStatus status, string entityId, string entityName = null, params object[] payload);
        Task SendLogAsync(string entity, Enum action, ActionStatus status, object entityId = null, string entityName = null, params object[] payload);
        Task SendLogAsync<T>(string entity, Enum action, ActionStatus status, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payload);

        Task SendLogAsync(string entity, Enum action, System.Exception exception, string entityId, string entityName = null, params object[] payload);
        Task SendLogAsync(string entity, Enum action, System.Exception exception, object entityId = null, string entityName = null, params object[] payload);
        Task SendLogAsync<T>(string entity, Enum action, System.Exception exception, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payload);

        Task SendLogAsync(Guid id, string entity, Enum action, System.Exception exception, string entityId, string entityName = null, params object[] payload);
        Task SendLogAsync(Guid id, string entity, Enum action, System.Exception exception, object entityId = null, string entityName = null, params object[] payload);
        Task SendLogAsync<T>(Guid id, string entity, Enum action, System.Exception exception, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payload);
    }
}