using System.IO;
using System.Collections.Generic;

namespace AHI.Infrastructure.Import.Abstraction
{
    public interface IFileHandler<T>
    {
        IEnumerable<T> Handle(Stream stream);
    }
}