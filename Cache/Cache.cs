using Microsoft.Extensions.Caching.Memory;

namespace MySqlEntityCore
{
    internal static class Cache
    {
        private static readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

        internal static dynamic Get(string key)
        {
            _memoryCache.TryGetValue(key, out dynamic memVal);
            return memVal;
        }

        internal static void Set(string key, object value, int keepSeconds = 0)
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

        internal static void Remove(string key) => _memoryCache.Remove(key);
    }
}
