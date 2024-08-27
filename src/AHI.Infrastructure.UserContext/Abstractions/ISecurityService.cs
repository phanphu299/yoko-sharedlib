using System.Collections.Generic;
namespace AHI.Infrastructure.UserContext.Service.Abstraction
{
    public interface ISecurityService
    {
        /// <summary>
        ///     Validate object base privileges.
        ///     Allow optionally validate role base privileges.
        /// </summary>
        /// <param name="includeRoleBase">
        ///     Include checking role base privileges before checking object base.<br/>
        ///     Normally, role base privileges should have been checked in authorization filter.
        ///     Only use for special cases (E.g. checking for additional required role base privilege that was not checked in the filter).
        /// </param>
        bool AuthorizeAccess(string applicationId, string entityCode, string privilegeCode, string resourcePath, string ownerUpn, bool throwException = true, bool includeRoleBase = false);
        //bool AuthorizeAccess(UserInfo userInfo, string entityCode, string privilegeCode, string resourcePath, string ownerUpn, bool throwException = true);
        IEnumerable<string> FindAllElementIds(string applicationId, string entityCode, string privilegeCode);
        IEnumerable<string> FindRestrictedElementIds(string applicationId, string entityCode, string privilegeCode);
        bool HasFullAccessPrivilege(string applicationId, string entityCode);
        bool HasChildAccessPrivilege(string applicationId, string entityCode, string privilegeCode, string resourceId);
    }
}
