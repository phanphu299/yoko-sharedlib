namespace AHI.Infrastructure.BlobStorage
{
    public class BlobStorageOptions
    {
        public string DefaultEndpointsProtocol { get; set; } = "https";
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string EndpointSuffix { get; set; } = "core.windows.net";
        public string DefaultContainer { get; set; }
        public string DefaultFileContainer { get; set; }
        public string ConnectionStringOverride { get; set; }
    }
}
