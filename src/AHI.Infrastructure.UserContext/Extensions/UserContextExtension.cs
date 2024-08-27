using System;
using System.Linq;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;

namespace AHI.Infrastructure.UserContext.Extension
{
    public static class UserContextExtension
    {
        public static IUserContext Clone(this IUserContext source)
        {
            var userContext = new UserContext.Internal.UserContext();
            userContext.SetRightShorts(source.RightShorts);
            // userContext.SetRightHashes(source.RightHashes);
            // userContext.SetObjectRightHashes(source.ObjectRightHashes);
            userContext.SetObjectRightShorts(source.ObjectRightShorts);
            userContext.SetId(source.Id);
            userContext.SetUpn(source.Upn);
            userContext.SetTimezone(source.Timezone);
            userContext.SetDateTimeFormat(source.DateTimeFormat);
            userContext.SetAvatar(source.Avatar);
            userContext.SetName(source.FirstName, source.MiddleName, source.LastName);
            return userContext;
        }
        public static void CopyTo(this IUserContext source, IUserContext target)
        {
            target.SetRightShorts(source.RightShorts);
            // target.SetRightHashes(source.RightHashes);
            // target.SetObjectRightHashes(source.ObjectRightHashes);
            target.SetObjectRightShorts(source.ObjectRightShorts);
            target.SetId(source.Id);
            target.SetUpn(source.Upn);
            target.SetTimezone(source.Timezone);
            target.SetDateTimeFormat(source.DateTimeFormat);
            target.SetAvatar(source.Avatar);
            target.SetName(source.FirstName, source.MiddleName, source.LastName);
        }

        public static bool ValidRoleBaseAccess<T>(this IUserContext userContext, ITenantContext tenantContext, ILoggerAdapter<T> logger, params Tuple<string, string, string>[] allowEntities)
        {
            var paths = allowEntities.SelectMany(allowEntity =>
            {
                var (applicationId, entityName, privilegeName) = allowEntity;
                return new[]{
                                $"t/{tenantContext.Tenant.Item1}/s/{tenantContext.Subscription.Item1}/a/{applicationId}/p/{tenantContext.Project.Item1}/e/{entityName}/o/*/p/{privilegeName}".ToLowerInvariant(),
                                $"t/{tenantContext.Tenant.Item1}/s/{tenantContext.Subscription.Item1}/a/{applicationId}/p/*/e/{entityName}/o/*/p/{privilegeName}".ToLowerInvariant(),
                                $"t/{tenantContext.Tenant.Item1}/s/*/a/{applicationId}/p/*/e/{entityName}/o/*/p/{privilegeName}".ToLowerInvariant(),
                                $"t/*/s/*/a/{applicationId}/p/*/e/{entityName}/o/*/p/{privilegeName}".ToLowerInvariant()
                };
            });
            logger.LogTrace($"Allow paths: {string.Join(",", paths)}");
            var restrictPaths = paths.Select(path => $"{path}/none");
            var restrictResult = (from restrictPath in restrictPaths
                                  join right in userContext.RightShorts on restrictPath equals right
                                  select restrictPath
                        );
            if (restrictResult.Any())
            {
                logger.LogDebug($"User right has been restricted. {string.Join(",", restrictResult)}");
                return false;
            }
            var result = (from path in paths
                          join right in userContext.RightShorts on path equals right
                          select path
                        );
            var checkResult = result.Any();
            logger.LogTrace($"Check result: {checkResult} ");
            if (!checkResult)
            {
                logger.LogTrace($"Hash check: {string.Join(",", result)}");
            }
            return checkResult;
        }
    }
}