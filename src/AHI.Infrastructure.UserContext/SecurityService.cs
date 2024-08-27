using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.UserContext.Service.Abstraction;
using AHI.Infrastructure.UserContext.Extension;

namespace AHI.Infrastructure.UserContext.Service
{
    public class SecurityService : ISecurityService
    {
        private readonly ITenantContext _tenantContext;
        private readonly IUserContext _userContext;
        private readonly ILoggerAdapter<SecurityService> _logger;
        public SecurityService(ITenantContext tenantContext, IUserContext userContext, ILoggerAdapter<SecurityService> logger)
        {
            _tenantContext = tenantContext;
            _userContext = userContext;
            _logger = logger;
        }

        public IEnumerable<string> FindAllElementIds(string applicationId, string entityCode, string privilegeCode)
        {
            var regexPattern = $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/(.+?)/p/{privilegeCode}".ToLowerInvariant();
            var matches = _userContext.ObjectRightShorts.Select(right => Regex.Match(right, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromMilliseconds(100)));
            var objectIds = matches.Select(match => match.Groups[1].Value);
            foreach (var item in objectIds)
            {
                var objectId = item.Split("/children/", 2)[0].Trim('/');
                if (!string.IsNullOrEmpty(objectId))
                {
                    yield return objectId;
                }
            }
        }
        public IEnumerable<string> FindRestrictedElementIds(string applicationId, string entityCode, string privilegeCode)
        {
            var regexPattern = $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/(.+?)/p/{privilegeCode}/none".ToLowerInvariant();
            var matches = _userContext.ObjectRightShorts.Select(right => Regex.Match(right, regexPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromMilliseconds(100)));
            var objectIds = matches.Select(match => match.Groups[1].Value);
            foreach (var item in objectIds)
            {
                var objectId = item.Split("/children/", 2)[0].Trim('/');
                if (!string.IsNullOrEmpty(objectId))
                {
                    yield return objectId;
                }
            }
        }

        public bool AuthorizeAccess(string applicationId, string entityCode, string privilegeCode, string resourcePath, string ownerUpn, bool throwException = true, bool includeRoleBase = false)
        {
            var authorizeResult = Authorize(applicationId, _userContext.Upn, entityCode, privilegeCode, resourcePath, ownerUpn, includeRoleBase);
            if (!authorizeResult && throwException)
            {
                throw new SystemSecurityException("Access denied");
            }
            return authorizeResult;
        }
        // public bool AuthorizeAccess(UserInfo userInfo, string entityCode, string privilegeCode, string resourcePath, string ownerUpn, bool throwException = true)
        // {
        //     var authorizeResult = Authorize(userInfo.ObjectRightShorts, userInfo.Upn, entityCode, privilegeCode, resourcePath, ownerUpn);
        //     if (!authorizeResult && throwException)
        //     {
        //         throw new SecurityViolationException("Access denied");
        //     }
        //     return authorizeResult;
        // }

        private bool Authorize(string applicationId, string upn, string entityCode, string privilegeCode, string resourcePath, string ownerUpn, bool includeRoleBase)
        {
            /*Check deny on parent level . 
                ResourcePath ex: "objects/ad8dfe0f-d8f4-4495-9b7c-e8cabe98fac4/children/f8dba0f1-5ece-4a01-aff8-7728615e497f/children/bb6471bb-3387-4d0b-b273-4c8af82f8e18"
                ObjectShorts: 
                    a0f1c338-1eff-40ff-997e-64f08e141b06/projects/34e5ee62-429c-4724-b3d0-3891bd0a08c9/entities/asset/objects/ad8dfe0f-d8f4-4495-9b7c-e8cabe98fac4/children/* /privileges/delete_asset/none.

                restrictIds should be :
                    entities/asset/objects/ad8dfe0f-d8f4-4495-9b7c-e8cabe98fac4/children/* /privileges/delete_asset/none
                    entities/asset/objects/f8dba0f1-5ece-4a01-aff8-7728615e497f/children/* /privileges/delete_asset/none
                    entities/asset/objects/bb6471bb-3387-4d0b-b273-4c8af82f8e18/children/* /privileges/delete_asset/none
                    entities/asset/objects/bb6471bb-3387-4d0b-b273-4c8af82f8e18/privileges/delete_asset/none

            */
            if (HasFullAccessPrivilege(applicationId, entityCode))
            {
                return true;
            }
            if (includeRoleBase)
            {
                var roleBaseResult = _userContext.ValidRoleBaseAccess(_tenantContext, _logger, Tuple.Create(applicationId, entityCode, privilegeCode));
                if (!roleBaseResult)
                    return false;
            }
            if (!string.IsNullOrEmpty(resourcePath) && resourcePath.Trim().StartsWith("objects/"))
            {
                var objectIds = resourcePath.Trim().Replace("objects/", "").Split("/children/");
                var restrictIds = new List<string>();
                restrictIds.AddRange(objectIds.Select(x => $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/{x}/children/*/p/{privilegeCode}/none".ToLowerInvariant()));

                var currentObjectId = objectIds.Last();
                restrictIds.Add($"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/{currentObjectId}/p/{privilegeCode}/none".ToLowerInvariant());

                var rightRestrictions = (
                    from restriction in restrictIds
                    join objectRightShort in _userContext.ObjectRightShorts on restriction equals objectRightShort
                    select restriction
                );
                if (rightRestrictions.Any())
                {
                    // deny access
                    _logger.LogTrace($"Deny from {string.Join(",", rightRestrictions)}");
                    return false;
                }
            }
            if (ownerUpn == upn)
            {
                _logger.LogTrace($"Granted from Owner - {ownerUpn} on path: {resourcePath}");
                return true;
            }

            //Check allow from parent to object. If user can access from parent it can see childs
            if (!string.IsNullOrEmpty(resourcePath) && resourcePath.Trim().StartsWith("objects/"))
            {
                var objectIds = resourcePath.Trim().Replace("objects/", "").Split("/children/");
                var allowedIds = new List<string>();
                allowedIds.AddRange(objectIds.Select(x => $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/{x}/children/*/p/{privilegeCode}".ToLowerInvariant()));

                var currentObjectId = objectIds.Last();
                allowedIds.Add($"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/{currentObjectId}/p/{privilegeCode}".ToLowerInvariant());

                var validRights = (
                                    from valid in allowedIds
                                    join objectRightShort in _userContext.ObjectRightShorts on valid equals objectRightShort
                                    select valid
                                );
                if (validRights.Any())
                {
                    // allow access
                    _logger.LogTrace($"Granted from {string.Join(",", validRights)}");
                    return true;
                }
            }
            return false;
        }

        public bool HasFullAccessPrivilege(string applicationId, string entityCode)
        {

            var entityFullAccessRights = new[] {
                $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/*/p/{entityCode}_full_access".ToLowerInvariant(),
                $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/*/e/{entityCode}/o/*/p/{entityCode}_full_access".ToLowerInvariant(),
                $"t/{_tenantContext.Tenant.Item1}/s/*/a/{applicationId}/p/*/e/{entityCode}/o/*/p/{entityCode}_full_access".ToLowerInvariant(),
                $"t/*/s/*/a/{applicationId}/p/*/e/{entityCode}/o/*/p/{entityCode}_full_access".ToLowerInvariant()
            };
            var fullAccessRights = (
                    from fullRight in entityFullAccessRights
                    join right in _userContext.RightShorts on fullRight equals right
                    select fullRight
                );
            if (fullAccessRights.Any())
            {
                _logger.LogTrace($"Granted from RBAC - {string.Join(",", fullAccessRights)}");
                return true;
            }
            return false;
        }

        public bool HasChildAccessPrivilege(string applicationId, string entityCode, string privilegeCode, string resourceId)
        {
            var childRightNone = $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/{resourceId}/children/*/p/{privilegeCode}/none".ToLowerInvariant();
            var hasNoneRight = _userContext.ObjectRightShorts.Contains(childRightNone);
            if (hasNoneRight)
            {
                // restricted
                return false;
            }
            var childRight = $"t/{_tenantContext.Tenant.Item1}/s/{_tenantContext.Subscription.Item1}/a/{applicationId}/p/{_tenantContext.Project.Item1}/e/{entityCode}/o/{resourceId}/children/*/p/{privilegeCode}".ToLowerInvariant();
            return _userContext.ObjectRightShorts.Contains(childRight);
        }
    }
}
