using System.Collections.Generic;
using System.Linq;
using AHI.Infrastructure.Import.Abstraction;

namespace AHI.Infrastructure.Import.ErrorTracking
{
    public class ImportTrackingService<T> : IImportTrackingService<T>
    {
        private readonly List<T> _errors = new List<T>();
        private readonly List<string> _files = new List<string>();
        public bool HasError => _errors.Any();

        public IEnumerable<T> Errors => _errors;

        public IEnumerable<string> Files => _files;

        public void Add(T error)
        {
            _errors.Add(error);
        }

        public void AddRange(IEnumerable<T> errors)
        {
            _errors.AddRange(errors);
        }

        public void TrackFiles(IEnumerable<string> files)
        {
            _files.AddRange(files);
        }
    }
}