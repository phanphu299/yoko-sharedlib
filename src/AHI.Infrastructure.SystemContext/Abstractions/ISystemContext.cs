using AHI.Infrastructure.SystemContext.Enum;

namespace AHI.Infrastructure.SystemContext.Abstraction
{
    public interface ISystemContext
    {
        ISystemContext SetAppLevel(AppLevel appLevel);
        AppLevel AppLevel { get; }
    }
}
