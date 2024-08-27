using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Import.Abstraction;

namespace AHI.Infrastructure.Import
{
    public abstract class BaseFileImport<T> : IFileImport
    {
        protected abstract IFileHandler<T> GetFileHandler();
        protected abstract IImportRepository<T> GetRepository();

        public virtual async Task ImportAsync(Stream stream)
        {
            var fileHandler = GetFileHandler();
            var repository = GetRepository();
            var dataTable = fileHandler.Handle(stream);
            if (dataTable != null && dataTable.Any())
            {
                await repository.CommitAsync(dataTable);
            }
        }

        public virtual async Task ImportAsync(Stream stream, Guid correlationId)
        {
            var fileHandler = GetFileHandler();
            var repository = GetRepository();
            var dataTable = fileHandler.Handle(stream);
            if (dataTable != null && dataTable.Any())
            {
                await repository.CommitAsync(dataTable, correlationId);
            }
        }
    }
}