using System;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SystemContext.Enum;

namespace AHI.Infrastructure.Audit.Model
{
    public abstract class ImportExportBasePayload
    {
        public Guid ActivityId { get; set; }

        protected readonly Enum _action;

        protected readonly ActionStatus _status;
        public virtual string Status => _status.ToString().ToLower();

        protected readonly string _objectName;
        public virtual string ObjectName => _objectName;

        public DateTime CreatedUtc { get; set; }
        public string Description { get; set; }

        protected ImportExportBasePayload(Guid activityId, string objectName, DateTime createdUtc, Enum action, ActionStatus status)
        {
            _action = action;
            _status = status;
            _objectName = objectName;

            ActivityId = activityId;
            CreatedUtc = createdUtc;
        }

        protected ImportExportBasePayload(ImportExportBasePayload source, Enum overrideAction = null)
        {
            _action = overrideAction ?? source._action;
            _status = source._status;
            _objectName = source._objectName;

            ActivityId = source.ActivityId;
            CreatedUtc = source.CreatedUtc;
            Description = source.Description;
        }

        public ActivityLogMessage CreateLog(string requestedBy, ITenantContext tenantContext, AppLevel appLevel)
        {
            return new ActivityLogMessage(ActivityId, _objectName, _action, _status, requestedBy, tenantContext, appLevel, new object[] { this });
        }
    }
}