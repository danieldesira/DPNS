using Enyim.Caching;
using Newtonsoft.Json;

namespace DPNS.Caching
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
    }

    public class CacheProvider : ICacheProvider
    {
        private readonly IMemcachedClient memcachedClient;

        public CacheProvider(IMemcachedClient memcachedClient) => this.memcachedClient = memcachedClient;

        public T Get<T>(string key)
        {
            return memcachedClient.Get<T>(key);
        }
    }
}
