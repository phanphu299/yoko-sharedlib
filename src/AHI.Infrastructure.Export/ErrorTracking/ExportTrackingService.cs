using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Model;

namespace AHI.Infrastructure.Export.ErrorTracking
{
    public class ExportTrackingService : BaseExportTrackingService
    {
        public ExportTrackingService()
        {
            _currentErrors = new List<TrackError>();
        }
    }
}