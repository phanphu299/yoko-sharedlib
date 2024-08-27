using System;
using System.Collections.Generic;
using System.IO;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Extension;

namespace AHI.Infrastructure.Audit.Model
{
    public class ImportExportLogPayload<TErrorDto> : ImportExportBasePayload
    {
        public override string Status => _status.ToStatusKey();
        public override string ObjectName => _objectName.ToEntityTypeKey();
        public int? Total { get; set; }
        public int? Success { get; set; }
        public int? Fail { get; set; }

        public string ActionType => _action.ToActionKey();
        public ICollection<ImportExportDetailPayload<TErrorDto>> Detail { get; set; }

        public ImportExportLogPayload(Guid activityId, string objectName, DateTime createdUtc, Enum action, ActionStatus status)
            : base(activityId, objectName, createdUtc, action, status)
        {
        }

        public ImportExportLogPayload(ImportExportNotifyPayload notifyPayload, Enum overrideAction = null) : base(notifyPayload, overrideAction)
        {
            switch (notifyPayload)
            {
                case ImportNotifyPayload importPayload:
                    Total = importPayload.Total;
                    Success = importPayload.Success;
                    Fail = importPayload.Fail;
                    break;
                case ExportNotifyPayload exportPayload:
                    Total = exportPayload.Total;
                    break;
            }
        }
    }

    public abstract class ImportExportDetailPayload<TErrorDto>
    {
        public string URL { get; set; }
        public string Status { get; set; }
        public int ErrorCount { get; set; }
        public ICollection<TErrorDto> Errors { get; set; }

        protected ImportExportDetailPayload(string url, ICollection<TErrorDto> errors)
        {
            URL = url;
            Errors = errors;
            ErrorCount = errors.Count;
            Status = (ErrorCount == 0 ? ActionStatus.Success : ActionStatus.Fail).ToStatusKey();
        }
    }

    public class ImportPayload<TErrorDto> : ImportExportDetailPayload<TErrorDto>
    {
        public string File { get; set; }

        public ImportPayload(string path, ICollection<TErrorDto> errors) : base(path, errors)
        {
            File = Path.GetFileName(path);
        }
    }

    public class ExportPayload<TErrorDto> : ImportExportDetailPayload<TErrorDto>
    {
        public ExportPayload(string path, ICollection<TErrorDto> errors) : base(path, errors)
        {
        }
    }
}