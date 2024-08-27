using System;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Extension;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SystemContext.Enum;

namespace AHI.Infrastructure.Audit.Model
{
    public class ActivityLogMessage : BusEvent
    {
        // private Guid activityId;
        // private string objectName;
        // private Enum action;
        // private ActionStatus status;
        // private ITenantContext tenantContext;
        // private AppLevel appLevel;
        // private object[] objects;

        public override string TopicName => "audit.application.log.created";

        public Guid Id { get; set; }
        public AppLevel ApplicationLevel { get; set; } = AppLevel.PROJECT;
        public string Entity { get; set; }
        public string EntityId { get; set; }
        public string EntityName { get; set; }

        public string Action { get; set; }
        public string Status { get; set; }
        public string RequestedBy { get; set; }

        public string TenantId { get; set; }
        public string ProjectId { get; set; }
        public string SubscriptionId { get; set; }
        public object[] Payloads { get; set; }
        [Obsolete("Should use Payloads instead")]
        public string[] Payload { get; set; }
        public PropertyName PropertyName { get; set; }

        public ActivityLogMessage()
        {
        }


        public ActivityLogMessage(
            Guid id, string entity, string entityId, string entityName,
            Enum action, ActionStatus status, string upn,
            ITenantContext tenantContext, AppLevel applicationLevel,
            object[] payloads = default)
        {
            Id = id;
            Entity = entity.ToEntityTypeKey();
            EntityId = entityId;
            EntityName = entityName;
            Action = action.ToActionKey();
            Status = status.ToStatusKey();
            RequestedBy = upn;

            TenantId = tenantContext.TenantId;
            SubscriptionId = tenantContext.SubscriptionId;
            ProjectId = tenantContext.ProjectId;

            ApplicationLevel = applicationLevel;

            Payloads = payloads;
        }

        public ActivityLogMessage(
            Guid id, string entity,
            Enum action, ActionStatus status, string upn,
            ITenantContext tenantContext, AppLevel applicationLevel,
            object[] payloads = default)
        : this(id, entity, null, null, action, status, upn, tenantContext, applicationLevel, payloads)
        {
        }

        public ActivityLogMessage(
            string entity, string entityId, string entityName,
            Enum action, ActionStatus status, string upn,
            ITenantContext tenantContext, AppLevel applicationLevel,
            object[] payloads = default)
        : this(Guid.NewGuid(), entity, entityId, entityName, action, status, upn, tenantContext, applicationLevel, payloads)
        {
        }

        public ActivityLogMessage(
            string entity,
            Enum action, ActionStatus status, string upn,
            ITenantContext tenantContext, AppLevel applicationLevel,
            object[] payloads = default)
        : this(Guid.NewGuid(), entity, null, null, action, status, upn, tenantContext, applicationLevel, payloads)
        {
        }
    }

    public class PropertyName
    {
        public string SubscriptionName { get; set; }
        public string ApplicationName { get; set; }
        public string ProjectName { get; set; }
    }
}