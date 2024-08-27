using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Constant;
using AHI.Infrastructure.Export.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;

namespace AHI.Infrastructure.Export.ErrorTracking
{
    public abstract class BaseExportTrackingService : IExportTrackingService
    {
        protected ICollection<TrackError> _currentErrors { get; set; }
        public ICollection<TrackError> GetErrors => _currentErrors;

        public bool HasError => (_currentErrors?.Count ?? -1) > 0;

        public virtual void RegisterError(string message, ErrorType errorType = ErrorType.UNDEFINED, IDictionary<string, object> validationInfo = null)
        {
            _currentErrors.Add(new TrackError(message, errorType, validationInfo));
        }
    }
}