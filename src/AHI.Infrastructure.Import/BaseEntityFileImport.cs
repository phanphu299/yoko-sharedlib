using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Import.Abstraction;

namespace AHI.Infrastructure.Import
{
    public abstract class BaseEntityFileImport<T> : IEntityFileImport<T>
    {
        protected abstract IFileHandler<T> GetFileHandler();
        protected abstract IEntityImportRepository<T> GetRepository();
        public virtual async Task ImportAsync(Guid entityId, Stream stream)
        {
            var fileHandler = GetFileHandler();
            var repository = GetRepository();
            var dataTable = fileHandler.Handle(stream);
            if (dataTable != null && dataTable.Any())
            {
                await repository.CommitAsync(entityId, dataTable);
                return;
            }
        }
    }
}