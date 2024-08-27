using System.Collections.Generic;

namespace AHI.Infrastructure.Import.Abstraction
{
    public interface IImportTrackingService<T>
    {
        void TrackFiles(IEnumerable<string> files);
        bool HasError { get; }
        IEnumerable<T> Errors { get; }
        void Add(T error);
        void AddRange(IEnumerable<T> errors);
        IEnumerable<string> Files { get; }
    }
}
