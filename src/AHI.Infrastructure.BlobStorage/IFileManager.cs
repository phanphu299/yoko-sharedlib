using System.IO;
using System.Threading.Tasks;

namespace AHI.Infrastructure.BlobStorage
{
    public interface IFileManager : IBlobManager
    {
        string FileContainerName { get; }
        // return a stream to download the file specified by name.
        Task<Stream> DownloadStreamAsync(string blobName);
    }
}