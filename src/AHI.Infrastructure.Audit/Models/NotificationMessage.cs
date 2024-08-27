using AHI.Infrastructure.MultiTenancy.Abstraction;

namespace AHI.Infrastructure.Audit.Model
{
    public abstract class NotificationMessage
    {
        public string Type { get; set; }
        public object Payload { get; set; }

        public NotificationMessage(string type, object payload)
        {
            Type = type;
            Payload = payload;
        }
    }

    public class TenantNotificationMessage : NotificationMessage
    {
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public string ApplicationId { get; set; }

        public TenantNotificationMessage(
            string type,
            string tenantId = null,
            string subscriptionId = null,
            string projectId = null,
            string applicationId = null,
            object payload = null)
        : base(type, payload)
        {
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
            ProjectId = projectId;
            ApplicationId = applicationId;
        }
        
        public TenantNotificationMessage(string type, ITenantContext tenantContext, string applicationId = null, object payload = null)
            : this(type, tenantContext.TenantId, tenantContext.SubscriptionId, tenantContext.ProjectId, applicationId, payload)
        {
        }
    }

    public class UpnNotificationMessage : NotificationMessage
    {
        public string Upn { get; set; }
        public string ApplicationId { get; set; }

        public UpnNotificationMessage(string type, string upn, object payload = null) : base(type, payload)
        {
            Upn = upn;
        }

        public UpnNotificationMessage(string type, string upn, string applicationId, object payload = null) : base(type, payload)
        {
            Upn = upn;
            ApplicationId = applicationId;
        }
    }
}