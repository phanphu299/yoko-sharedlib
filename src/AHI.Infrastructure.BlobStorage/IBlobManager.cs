using System.Threading.Tasks;

namespace AHI.Infrastructure.BlobStorage
{
    public interface IBlobManager
    {
        string StorageAccountName { get; }
        string StorageEndpoint { get; }
        string BlobContainerName { get; }
        Task CreateContainerIfNotExistsAsync();
        Task<string> DownloadBlobAsync(string blobName);
        Task<string> UploadBlobAsync(string fileName, byte[] fileContent, string contentType = null);
        Task<string> CopyBlobAsync(string fileNameSource, string fileNameDesc);

        // Grant access to specific blob in the container. The SAS valid with scope is blob.
        string GrantAccess(string fileName, int durationInSecond = 300);
        // Grant access to container. The SAS valid with the scope is container.
        string GrantAccess(int durationInSecond = 300);
    }
}
