using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

namespace AHI.Infrastructure.BlobStorage.Internal
{
    public class FileStorageManager : BlobStorageManager, IFileManager
    {
        private readonly CloudBlobContainer _fileContainer;
        public FileStorageManager(BlobStorageOptions option) : this(option, option.DefaultFileContainer)
        {
        }
        public FileStorageManager(BlobStorageOptions option, string fileContainer) : base(option, option.DefaultContainer)
        {
            fileContainer = fileContainer ?? option.DefaultFileContainer;
            var client = _storageAccount.CreateCloudBlobClient();
            _fileContainer = client.GetContainerReference(fileContainer);
        }
        public string FileContainerName => _fileContainer.Name;

        // get a SAS token with specific permission for download file from Blob Container.
        // fileName should be in the form of {subpath}/filename where subpath start right after container name
        public override string GrantAccess(string fileName, int durationInSecond = 300)
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(durationInSecond),
            };
            var blobPath = _fileContainer.GetBlockBlobReference(fileName);
            var sasToken = blobPath.GetSharedAccessSignature(policy);
            return sasToken;
        }

        // get a SAS token with specific permission for uploading file to Blob Container.
        public override string GrantAccess(int durationInSecond = 300)
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Create | SharedAccessBlobPermissions.Write,
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(durationInSecond)
            };
            return _fileContainer.GetSharedAccessSignature(policy);
        }

        // return a stream to download the file specified by name.
        public async Task<System.IO.Stream> DownloadStreamAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentNullException("Blob name can't be null.");

            var blockBlob = _fileContainer.GetBlockBlobReference(blobName);
            if (await blockBlob.ExistsAsync())
            {
                // set minimum number of bytes to buffer, with minimum value is at least 16KB
                // blockBlob.StreamMinimumReadSizeInBytes = 16 * 1024;
                return await blockBlob.OpenReadAsync();
            }
            else
            {
                throw new Exception("Blob not found.");
            }
        }
    }
}