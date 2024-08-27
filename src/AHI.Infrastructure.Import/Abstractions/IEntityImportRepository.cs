using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace AHI.Infrastructure.Import.Abstraction
{
    // repository interface for importing data
    // IMPORTANT: required to implement
    public interface IEntityImportRepository<T>
    {
        Task CommitAsync(Guid entityId, IEnumerable<T> source);
    }
}