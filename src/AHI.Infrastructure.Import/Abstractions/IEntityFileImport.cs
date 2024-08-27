using System;
using System.IO;
using System.Threading.Tasks;

namespace AHI.Infrastructure.Import.Abstraction
{
    public interface IEntityFileImport<T>
    {
        Task ImportAsync(Guid entityId, Stream stream);
    }
}