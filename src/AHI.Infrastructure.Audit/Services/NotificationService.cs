using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Extension;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;

namespace AHI.Infrastructure.Audit.Service
{
    public class NotificationService : INotificationService
    {
        protected virtual bool ShouldSerializePayload { get; } = true;

        protected readonly IHttpClientFactory _clientFactory;
        protected readonly ILoggerAdapter<NotificationService> _logger;
        private readonly ITenantContext _tenantContext;

        public NotificationService(IHttpClientFactory clientFactory, ILoggerAdapter<NotificationService> logger, ITenantContext tenantContext)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _tenantContext = tenantContext;
        }

        public virtual async Task SendNotifyAsync(string endpoint, NotificationMessage message)
        {
            if (ShouldSerializePayload && !(message.Payload is string))
            {
                message.Payload = message.Payload.ToJson();
            }
            var httpClient = _clientFactory.CreateClient(ClientNameConstant.NOTIFICATION_HUB, _tenantContext);
            var response = await httpClient.PostAsync(endpoint, message.ToJsonStringContent());
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogError(content);
            }
        }
    }
}