using System;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.SystemContext.Enum;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.UserContext.Extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AHI.Infrastructure.Audit.Extension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddNotification(this IServiceCollection services)
        {
            services.AddHttpClient(ClientNameConstant.NOTIFICATION_HUB, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["NotificationHubEndpoint"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();
            services.AddScoped<INotificationService, NotificationService>();
            return services;
        }

        public static IServiceCollection AddAuditLogService(this IServiceCollection services, AppLevel defaultAppLevel = AppLevel.PROJECT, bool allowSystemExceptionMessage = false)
        {
            services.AddMultiTenantService();
            services.AddUserContextService();
            services.AddScoped<IAuditLogService>(provider => {
                var tenantContext = provider.GetRequiredService<ITenantContext>();
                var userContext = provider.GetRequiredService<IUserContext>();
                var auditService = new AuditLogService(provider, tenantContext, userContext);
                auditService.SetAppLevel(defaultAppLevel);
                auditService.SetAllowSystemExceptionMessage(allowSystemExceptionMessage);
                return auditService;
            });
            return services;
        }
    }
}