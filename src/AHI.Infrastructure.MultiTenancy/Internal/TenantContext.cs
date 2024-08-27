using System;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Extension;

namespace AHI.Infrastructure.MultiTenancy.Internal
{
    public class TenantContext : ITenantContext
    {
        private string _projectId;
        private string _subscriptionId;
        private string _tenantId;
        public string SubscriptionId => _subscriptionId;
        public string ProjectId => _projectId;
        public string TenantId => _tenantId;

        public (Guid, int?) Tenant => TenantContextExtension.Parse(_tenantId);

        public (Guid, int?) Subscription => TenantContextExtension.Parse(_subscriptionId);

        public (Guid, int?) Project => TenantContextExtension.Parse(_projectId);

        public ITenantContext SetProjectId(string projectId)
        {
            _projectId = projectId;
            return this;
        }

        public ITenantContext SetSubscriptionId(string subscriptionId)
        {
            _subscriptionId = subscriptionId;
            return this;
        }
        public ITenantContext SetTenantId(string tenantId)
        {
            _tenantId = tenantId;
            return this;
        }
    }
}