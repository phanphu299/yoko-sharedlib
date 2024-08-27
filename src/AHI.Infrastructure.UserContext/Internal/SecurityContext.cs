using System.Collections.Generic;
using System.Linq;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.UserContext.Service.Abstraction;

namespace AHI.Infrastructure.UserContext.Internal
{
    public class SecurityContext : ISecurityContext
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityService _securityService;
        public SecurityContext(IUserContext userContext, ISecurityService securityService)
        {
            _userContext = userContext;
            _securityService = securityService;
        }
        private bool _fullAccess;
        public bool FullAccess => _fullAccess;
        private IEnumerable<string> _restrictedIds;
        public IEnumerable<string> RestrictedIds => _restrictedIds;
        private IEnumerable<string> _allowedIds;
        public IEnumerable<string> AllowedIds => _allowedIds;
        private string _upn;
        public string Upn => _upn;

        public ISecurityContext Authorize(string applicationId, string entityCode, string privilegeCode)
        {
            _fullAccess = _securityService.HasFullAccessPrivilege(applicationId, entityCode);
            _restrictedIds = _securityService.FindRestrictedElementIds(applicationId, entityCode, privilegeCode);
            _allowedIds = _securityService.FindAllElementIds(applicationId, entityCode, privilegeCode).Except(_restrictedIds);
            _upn = _userContext.Upn;
            return this;
        }

        public ISecurityContext SetFullAccess(bool fullAccess)
        {
            _fullAccess = fullAccess;
            return this;
        }

        public ISecurityContext SetRestrictedIds(IEnumerable<string> restrictedIds)
        {
            _restrictedIds = restrictedIds;
            return this;
        }

        public ISecurityContext SetAllowedIds(IEnumerable<string> allowedIds)
        {
            _allowedIds = allowedIds;
            return this;
        }

        public ISecurityContext SetUpn(string upn)
        {
            _upn = upn;
            return this;
        }
    }
}