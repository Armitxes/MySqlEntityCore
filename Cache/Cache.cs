using Microsoft.Extensions.Caching.Memory;

namespace MySqlEntityCore
{
    public static class Cache
    {
        private static MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        public static dynamic Get(string key)
        {
            _memoryCache.TryGetValue(key, out dynamic memVal);
            return memVal;
        }

        public static void Set(string key, object value, int keepSeconds = 0)
        {
            if (keepSeconds == 0)
            {
                _memoryCache.Set(key: key, value: value);
                return;
            }

            _memoryCache.Set(
                key: key,
                absoluteExpiration: System.DateTimeOffset.UtcNow.AddSeconds(keepSeconds),
                value: value
            );
        }

        public static void Clear()
        {
            _memoryCache.Dispose();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
        }

        public static void Remove(string key) => _memoryCache.Remove(key);
    }
}
