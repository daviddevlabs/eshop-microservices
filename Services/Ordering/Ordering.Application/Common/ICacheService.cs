namespace Ordering.Application.Common;

public interface ICacheService
{
    Task<string?> GetAsync(string key);

    Task<Dictionary<string, string?>> GetMultipleAsync(IEnumerable<string> keys);

    Task SetAsync(string key, string value, TimeSpan expiration);
}