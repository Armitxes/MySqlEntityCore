using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

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
            _memoryCache.Set(key, value);
            if (keepSeconds > 0)
                Task.Delay(keepSeconds * 1000).ContinueWith((t) => Remove(key));
        }

        internal static void Remove(string key) => _memoryCache.Remove(key);
    }
}
