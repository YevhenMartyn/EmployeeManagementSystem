
namespace BusinessLogicLayer.Interface
{
    public interface ICacheService<T> where T : class
    {
        Task<T> GetCacheAsync<T>(string cacheKey);
        Task SetCacheAsync<T>(string cacheKey, T value);
        Task InvalidateCacheAsync(int id);
        string GetCacheKey(string s);
    }
}
