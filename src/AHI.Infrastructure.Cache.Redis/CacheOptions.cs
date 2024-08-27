namespace AHI.Infrastructure.Cache.Redis
{
    public class CacheOptions
    {
        public string Host { get; set; }
        public string Password { get; set; }
        public bool Ssl { get; set; } = true;
        public bool AbortOnConnectFail { get; set; } = false;
        public int CacheDuration { get; set; } = 60 * 60 * 24; // 1 day
        public int Database { get; set; } = -1;
        public int SyncTimeout { get; set; } = 500;
        public int AsyncTimeout { get; set; } = 500;
        public int ConnectTimeout { get; set; } = 500;
        public int PageSize { get; set; } = 100000;
        public bool ClusterEnabled { get; set; }
        public string EndPoints { get; set; }
    }
}
