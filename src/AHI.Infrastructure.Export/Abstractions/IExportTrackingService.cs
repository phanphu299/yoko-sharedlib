using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using System.Collections.Generic;

namespace AHI.Infrastructure.Export.Abstraction
{
    public interface IExportTrackingService : IErrorService
    {
        ICollection<TrackError> GetErrors { get; }
    }
}
