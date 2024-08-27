using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace AHI.Infrastructure.BlobStorage.Internal
{
    public class BlobStorageManager : IBlobManager
    {
        private readonly CloudBlobContainer _container;
        protected readonly CloudStorageAccount _storageAccount;
        public BlobStorageManager(BlobStorageOptions option) : this(option, option.DefaultContainer)
        {
        }
        public BlobStorageManager(BlobStorageOptions option, string container)
        {
            container = container ?? option.DefaultContainer;
            // Get azure table storage connection string.
            string host = option.ConnectionStringOverride;
            if (string.IsNullOrEmpty(host))
            {
                host = "DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2};EndpointSuffix={3}";

                host = string.Format(host,
                    option.DefaultEndpointsProtocol,
                    option.AccountName,
                    option.AccountKey,
                    option.EndpointSuffix);
            }
            _storageAccount = CloudStorageAccount.Parse(host);
            var client = _storageAccount.CreateCloudBlobClient();
            _container = client.GetContainerReference(container);
        }
        public virtual string StorageAccountName => _storageAccount.Credentials.AccountName;
        public virtual string StorageEndpoint => _storageAccount.BlobEndpoint.AbsoluteUri;
        public virtual string BlobContainerName => _container.Name;

        public virtual string GrantAccess(string fileName, int durationInSecond = 300)
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(durationInSecond),
            };
            var blobPath = _container.GetBlockBlobReference(fileName);
            var sasToken = blobPath.GetSharedAccessSignature(policy);
            return sasToken;
        }

        public virtual string GrantAccess(int durationInSecond = 300)
        {
            // Create a new access policy for the account.
            SharedAccessAccountPolicy policy = new SharedAccessAccountPolicy()
            {
                Permissions = SharedAccessAccountPermissions.Read,
                Services = SharedAccessAccountServices.Blob,
                ResourceTypes = SharedAccessAccountResourceTypes.Object,
                SharedAccessExpiryTime = DateTime.UtcNow.AddSeconds(durationInSecond),
                Protocols = SharedAccessProtocol.HttpsOnly
            };
            // Return the SAS token.
            return _storageAccount.GetSharedAccessSignature(policy);
        }

        public async Task<string> DownloadBlobAsync(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentNullException("Blob name can't be null.");

            var blockBlob = _container.GetBlockBlobReference(blobName);
            if (await blockBlob.ExistsAsync())
            {
                return await blockBlob.DownloadTextAsync();
            }
            else
            {
                throw new Exception("Blob not found.");
            }
        }

        public async Task<string> UploadBlobAsync(string fileName, byte[] fileContent, string contentType = null)
        {
            // Check HttpPostedFileBase is null or not
            if (fileContent == null || fileContent.Length == 0)
                throw new ArgumentNullException("FileContent can't be null.");

            // Create a block blob
            //var blockBlob = blobContainer.GetBlockBlobReference(fileToUpload.FileName);
            var blockBlob = _container.GetBlockBlobReference(fileName);

            // upload to blob
            await blockBlob.UploadFromByteArrayAsync(fileContent, 0, fileContent.Length);

            if (contentType != null)
            {
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.SetPropertiesAsync();
            }
            // get file uri
            return blockBlob.Uri.AbsoluteUri;
        }

        public async Task<string> CopyBlobAsync(string fileNameSource, string newPathWithFileName)
        {
            if (fileNameSource.Equals(newPathWithFileName))
                throw new ArgumentNullException("fileNameSource can't be the same with newPathWithFileName.");

            if (string.IsNullOrEmpty(fileNameSource) || string.IsNullOrEmpty(newPathWithFileName))
                throw new ArgumentNullException("fileNameSource & newPathWithFileName can't be null.");

            var sourceBlob = _container.GetBlockBlobReference(fileNameSource);
            var descBlob = _container.GetBlockBlobReference(newPathWithFileName);
            await descBlob.StartCopyAsync(sourceBlob);
            return newPathWithFileName;
        }

        public Task CreateContainerIfNotExistsAsync()
        {
            return _container.CreateIfNotExistsAsync();
        }
    }
}
