using AHI.Infrastructure.UserContext.Service.Abstraction;

namespace AHI.Infrastructure.UserContext.Extension
{
    public static class SecurityContextExtension
    {
        public static void CopyTo(this ISecurityContext source, ISecurityContext target)
        {
            target.SetFullAccess(source.FullAccess);
            target.SetRestrictedIds(source.RestrictedIds);
            target.SetAllowedIds(source.AllowedIds);
            target.SetUpn(source.Upn);
        }
    }
}