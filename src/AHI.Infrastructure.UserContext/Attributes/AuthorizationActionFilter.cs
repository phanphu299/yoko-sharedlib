using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using System;
using AHI.Infrastructure.UserContext.Extension;

namespace AHI.Infrastructure.Authorization
{
    public class RightsAuthorizeFilterAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Tuple<string, string, string>[] _allowEntities;
        public RightsAuthorizeFilterAttribute(params string[] allowEntities)
        {
            _allowEntities = allowEntities.Select(right =>
            {
                var rights = right.Split('/', 3, StringSplitOptions.RemoveEmptyEntries);
                return Tuple.Create(rights[0], rights[1], rights[2]);
            }).ToArray();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthorized = CheckUserPermission(context.HttpContext);
            if (!isAuthorized)
            {
                context.Result = new ForbidResult("oidc");
            }
        }

        private bool CheckUserPermission(HttpContext context)
        {
            var user = context.User;
            if (user.Identity.IsAuthenticated)
            {
                return ValidPathAccess(context);
            }
            return false;
        }

        private bool ValidPathAccess(HttpContext context)
        {
            var logger = context.RequestServices.GetService(typeof(ILoggerAdapter<RightsAuthorizeFilterAttribute>)) as ILoggerAdapter<RightsAuthorizeFilterAttribute>;
            var userContext = context.RequestServices.GetService(typeof(IUserContext)) as IUserContext;
            var tenantContext = context.RequestServices.GetService(typeof(ITenantContext)) as ITenantContext;
            return userContext.ValidRoleBaseAccess(tenantContext, logger, _allowEntities);
        }
    }
}