using System;

namespace AHI.Infrastructure.MultiTenancy.Abstraction
{
    public interface ITenantContext
    {
        ITenantContext SetTenantId(string tenantId);
        string TenantId { get; }
        (Guid, int?) Tenant { get; }
        ITenantContext SetSubscriptionId(string subscriptionId);
        (Guid, int?) Subscription { get; }
        string SubscriptionId { get; }
        ITenantContext SetProjectId(string projectId);
        (Guid, int?) Project { get; }
        string ProjectId { get; }
    }
}
