using AHI.Infrastructure.SystemContext.Abstraction;
using AHI.Infrastructure.SystemContext.Enum;

namespace AHI.Infrastructure.SystemContext.Internal
{
    public class SystemContext : ISystemContext
    {
        private AppLevel _appLevel = AppLevel.PROJECT;
        public ISystemContext SetAppLevel(AppLevel appLevel)
        {
            _appLevel = appLevel;
            return this;
        }
        public AppLevel AppLevel => _appLevel;
    }
}