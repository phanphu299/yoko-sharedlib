using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SystemContext.Enum;
using AHI.Infrastructure.UserContext.Abstraction;

namespace AHI.Infrastructure.Audit.Service
{
    public class AuditLogService : IAuditLogService
    {
        public AppLevel AppLevel { get; private set; } = AppLevel.PROJECT;

        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;

        private PropertyName _propertyName;

        // allow log message from Exceptions that are not derived from AHI.Infrastructure.Exception.BaseException
        private bool _allowSystemExceptionMessage = false;

        public AuditLogService(IServiceProvider serviceProvider, ITenantContext tenantContext, IUserContext userContext)
        {
            _serviceProvider = serviceProvider;
            _tenantContext = tenantContext;
            _userContext = userContext;
        }

        public void SetAppLevel(AppLevel appLevel)
        {
            AppLevel = appLevel;
        }

        public void SetAllowSystemExceptionMessage(bool allowSystemExceptionMessage)
        {
            _allowSystemExceptionMessage = allowSystemExceptionMessage;
        }

        public void SetPropertyName(string applicationName, string subscriptionName = null, string projectName = null)
        {
            _propertyName = new PropertyName
            {
                ApplicationName = applicationName,
                SubscriptionName = subscriptionName,
                ProjectName = projectName,
            };
        }

        public Task SendLogAsync(ActivityLogMessage message)
        {
            message.PropertyName = _propertyName;
            var dispatcher = _serviceProvider.GetService(typeof(IDomainEventDispatcher)) as IDomainEventDispatcher;
            return dispatcher.SendAsync(message);
        }

        public Task SendLogAsync(Guid id, string entity, Enum action, ActionStatus status, string entityId, string entityName = null, params object[] payloads)
        {
            var message = new ActivityLogMessage(id, entity, entityId, entityName, action, status, _userContext.Upn, _tenantContext, AppLevel, payloads);
            return SendLogAsync(message);
        }

        public Task SendLogAsync(Guid id, string entity, Enum action, ActionStatus status, object entityId = null, string entityName = null, params object[] payload)
        {
            return SendLogAsync(id, entity, action, status, entityId?.ToString(), entityName, payload);
        }

        public Task SendLogAsync<T>(Guid id, string entity, Enum action, ActionStatus status, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payloads)
        {
            var entityId = CombineData(entityIds);
            var entityName = CombineData(entityNames);
            var message = new ActivityLogMessage(id, entity, entityId, entityName, action, status, _userContext.Upn, _tenantContext, AppLevel, payloads);
            return SendLogAsync(message);
        }

        public Task SendLogAsync(string entity, Enum action, ActionStatus status, string entityId, string entityName = null, params object[] payloads)
        {
            var message = new ActivityLogMessage(entity, entityId, entityName, action, status, _userContext.Upn, _tenantContext, AppLevel, payloads);
            return SendLogAsync(message);
        }

        public Task SendLogAsync(string entity, Enum action, ActionStatus status, object entityId = null, string entityName = null, params object[] payloads)
        {
            return SendLogAsync(entity, action, status, entityId?.ToString(), entityName, payloads);
        }

        public Task SendLogAsync<T>(string entity, Enum action, ActionStatus status, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payloads)
        {
            var entityId = CombineData(entityIds);
            var entityName = CombineData(entityNames);
            var message = new ActivityLogMessage(entity, entityId, entityName, action, status, _userContext.Upn, _tenantContext, AppLevel, payloads);
            return SendLogAsync(message);
        }

        public Task SendLogAsync(string entity, Enum action, System.Exception exception, string entityId, string entityName = null, params object[] payload)
        {
            payload = ParseExceptionPayload(exception, payload);
            var status = ActionStatus.Fail;
            return SendLogAsync(entity, action, status, entityId, entityName, payload);
        }

        public Task SendLogAsync(string entity, Enum action, System.Exception exception, object entityId = null, string entityName = null, params object[] payload)
        {
            return SendLogAsync(entity, action, exception, entityId?.ToString(), entityName, payload);
        }

        public Task SendLogAsync<T>(string entity, Enum action, System.Exception exception, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payload)
        {
            payload = ParseExceptionPayload(exception, payload);
            var status = ActionStatus.Fail;
            return SendLogAsync(entity, action, status, entityIds, entityNames, payload);
        }

        public Task SendLogAsync(Guid id, string entity, Enum action, System.Exception exception, string entityId, string entityName = null, params object[] payload)
        {
            payload = ParseExceptionPayload(exception, payload);
            var status = ActionStatus.Fail;
            return SendLogAsync(id, entity, action, status, entityId, entityName, payload);
        }

        public Task SendLogAsync(Guid id, string entity, Enum action, System.Exception exception, object entityId = null, string entityName = null, params object[] payload)
        {
            return SendLogAsync(id, entity, action, exception, entityId?.ToString(), entityName, payload);
        }

        public Task SendLogAsync<T>(Guid id, string entity, Enum action, System.Exception exception, IEnumerable<T> entityIds, IEnumerable<string> entityNames = null, params object[] payload)
        {
            payload = ParseExceptionPayload(exception, payload);
            var status = ActionStatus.Fail;
            return SendLogAsync(id, entity, action, status, entityIds, entityNames, payload);
        }

        // private string[] ParsePayload(object[] payload)
        // {
        //     if (payload is null || !payload.Any())
        //         return null;

        //     return payload.Select(x => (x is string) ? (x as string) : x?.JsonSerialize()).ToArray();
        // }

        private object[] ParseExceptionPayload(System.Exception exception, object[] payload)
        {
            object exceptionPayload;
            switch (exception)
            {
                case EntityValidationException validationException:
                    exceptionPayload = new
                    {
                        ErrorCode = validationException.ErrorCode,
                        DetailCode = validationException.DetailCode,
                        Fields = validationException.Failures.SelectMany(fieldFailure => fieldFailure.Value.Select(item => new
                        {
                            Name = fieldFailure.Key,
                            ErrorMessage = item,
                            ValidationInfo = validationException.ValidationInfo.ContainsKey(fieldFailure.Key)
                                                ? validationException.ValidationInfo[fieldFailure.Key]
                                                : null
                        }))
                    };
                    break;
                case BaseException baseException:
                    exceptionPayload = new
                    {
                        ErrorCode = baseException.ErrorCode,
                        DetailCode = baseException.DetailCode
                    };
                    break;
                default:
                    exceptionPayload = new
                    {
                        Message = $"{exception.GetType().Name} {(_allowSystemExceptionMessage ? exception.Message : null)}".Trim()
                    };
                    break;
            };
            var newPayloads = new List<object> { exceptionPayload };
            newPayloads.AddRange(payload);
            return newPayloads.ToArray();
        }

        private const string SEPERATOR = ", ";
        private string CombineData<T>(IEnumerable<T> data)
        {
            if (data is null || !data.Any())
                return null;

            var joinData = string.Join(SEPERATOR, data);
            var length = Math.Min(510, joinData.Length);
            return joinData.Substring(0, length);
        }
    }
}