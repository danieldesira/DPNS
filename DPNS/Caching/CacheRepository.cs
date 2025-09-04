using Enyim.Caching;

namespace DPNS.Caching
{
    public interface ICacheRepository
    {
        void Set<T>(string key, T value);
    }

    public class CacheRepository : ICacheRepository
    {
        private readonly IMemcachedClient memcachedClient;

        public CacheRepository(IMemcachedClient memcachedClient) => this.memcachedClient = memcachedClient;

        public void Set<T>(string key, T value)
        {
            memcachedClient.Set(key, value, 3600);
        }
    }
}
