using System;
using AHI.Infrastructure.Audit.Constant;

namespace AHI.Infrastructure.Audit.Extension
{
    public static class CommonAuditExtension
    {
        private const string AUDIT_LOG_STATUS_PREFIX = "AUDIT.LOG.STATUS.{0}";
        private const string AUDIT_LOG_ACTION_PREFIX = "AUDIT.LOG.ACTION.{0}";
        private const string AUDIT_LOG_ENTITY_TYPE_PREFIX = "AUDIT.LOG.ENTITY_TYPE.{0}";

        public static string ToStatusKey(this ActionStatus status) => string.Format(AUDIT_LOG_STATUS_PREFIX, status.ToString()).ToUpper();
        public static string ToActionKey(this Enum action) => string.Format(AUDIT_LOG_ACTION_PREFIX, action.ToString()).ToUpper();
        public static string ToEntityTypeKey(this string entityType) => string.Format(AUDIT_LOG_ENTITY_TYPE_PREFIX, entityType.Replace(' ', '_')).ToUpper();
    }
}