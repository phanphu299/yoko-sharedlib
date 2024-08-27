using System;
using System.IO;
using System.Threading.Tasks;

namespace AHI.Infrastructure.Import.Abstraction
{
    public interface IFileImport
    {
        Task ImportAsync(Stream stream);
        Task ImportAsync(Stream stream, Guid correlationId);
    }
}