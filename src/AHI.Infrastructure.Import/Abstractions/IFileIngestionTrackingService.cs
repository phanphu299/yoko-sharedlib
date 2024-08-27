using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Constant;
using AHI.Infrastructure.SharedKernel.Model;

namespace AHI.Infrastructure.Import.Abstraction
{
    public interface IFileIngestionTrackingService : IErrorService
    {
        ICollection<TrackError> GetErrors { get; }
        void RegisterError(string message, int rowIndex, string key = "", ErrorType errorType = ErrorType.VALIDATING);
    }
}
