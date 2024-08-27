using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace AHI.Infrastructure.Import.Abstraction
{
    // repository interface for importing data
    // IMPORTANT: required to implement
    public interface IImportRepository<T>
    {
        Task CommitAsync(IEnumerable<T> source);
        Task CommitAsync(IEnumerable<T> source, Guid correlationId);
    }
}