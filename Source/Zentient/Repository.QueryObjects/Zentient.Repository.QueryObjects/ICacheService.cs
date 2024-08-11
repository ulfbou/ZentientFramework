using Microsoft.Extensions.Caching.Memory;

namespace Zentient.Repository.QueryObjects
{
    public interface ICacheService
    {
        public IEnumerable<T> GetOrSet<T>(string cacheKey, Func<IEnumerable<T>> getItemCallback, TimeSpan expiration) where T : class;
    }

    public class MemoryCacheService : ICacheService
    {
        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public IEnumerable<T> GetOrSet<T>(string cacheKey, Func<IEnumerable<T>> getItemCallback, TimeSpan expiration) where T : class
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<T> item))
            {
                item = getItemCallback();
                if (item == null)
                {
                    _cache.Set(cacheKey, item, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiration
                    });
                }
            }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            return item ?? Enumerable.Empty<T>();
        }
    }
}