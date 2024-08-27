using System.Collections.Generic;

namespace AHI.Infrastructure.UserContext.Service.Abstraction
{
    public interface ISecurityContext
    {
        ISecurityContext Authorize(string applicationId, string entityCode, string privilegeCode);
        public bool FullAccess { get; }
        public IEnumerable<string> RestrictedIds { get; }
        public IEnumerable<string> AllowedIds { get; }
        public string Upn { get; }
        ISecurityContext SetFullAccess(bool fullAccess);
        ISecurityContext SetRestrictedIds(IEnumerable<string> restrictedIds);
        ISecurityContext SetAllowedIds(IEnumerable<string> allowedIds);
        ISecurityContext SetUpn(string upn);
    }
}
