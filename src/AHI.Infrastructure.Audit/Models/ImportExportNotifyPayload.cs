using System;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Extension;

namespace AHI.Infrastructure.Audit.Model
{
    public abstract class ImportExportNotifyPayload : ImportExportBasePayload
    {
        public string NotifyType => _action.ToString().ToLower();
        public override string ObjectName => _objectName.ToEntityTypeKey();

        protected ImportExportNotifyPayload(Guid activityId, string objectName, DateTime createdUtc, Enum action, ActionStatus status)
            : base(activityId, objectName, createdUtc, action, status)
        {
        }
    }

    public class ImportNotifyPayload : ImportExportNotifyPayload
    {
        public int? Total { get; set; }
        public int? Success { get; set; }
        public int? Fail { get; set; }

        public ImportNotifyPayload(Guid activityId, string objectName, DateTime createdUtc, Enum action, ActionStatus status)
            : base(activityId, objectName, createdUtc, action, status)
        {
        }
    }

    public class ExportNotifyPayload : ImportExportNotifyPayload
    {
        public int? Total { get; set; }
        public string URL { get; set; }

        public ExportNotifyPayload(Guid activityId, string objectName, DateTime createdUtc, Enum action, ActionStatus status)
            : base(activityId, objectName, createdUtc, action, status)
        {
        }
    }
}